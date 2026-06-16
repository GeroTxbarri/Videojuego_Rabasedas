using UnityEngine;
using System.Collections;

public class Movimiento_jugador : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;

    [Header("Movimiento Base")]
    public float fuerzaMovimiento = 20f;
    public float velocidadMaxima = 5f;

    [Header("Salto Cargado")]
    [Tooltip("Fuerza máxima al cargar el salto al 100%")]
    public float fuerzaSaltoMaxima = 15f;
    [Tooltip("Velocidad a la que crece la barra de carga (unidades/seg, 1 = llena en 1 seg)")]
    public float velocidadCarga = 0.5f;

    [Header("Detección de Piso")]
    public float radioDeteccion = 0.3f;
    public LayerMask capasPiso = ~0;
    public float offsetSuelo = 1f;

    // --- Variables de Estado ---
    public bool tocaPiso; 
    private bool paralizado = false;
    public bool IsParalizado => paralizado;

    // --- Lógica de Input (Para separar Update de FixedUpdate) ---
    private float inputH = 0f;
    private float inputV = 0f;

    // --- Lógica de carga (click izquierdo) ---
    private bool clickPresionado = false;
    private float tiempoPresionado = 0f;
    private bool modoCargar = false;
    private float cargaActual = 0f; 
    private bool saltoCargadoCancelado = false;
    private const float tiempoActivacionCarga = 0.3f;

    // --- Variables visuales (Parálisis) ---
    private Color colorOriginal = Color.white;
    private Renderer meshRenderer;
    private Renderer[] allRenderers;
    private Color[][] originalColors;
    private Color[][] originalEmissionColors;
    private bool[][] originalEmissionEnabled;

    void Start()
    {
        // Busca el Animator en tu modelo 3D (el hijo)
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        InicializarColores();
    }

    void Update()
    {
        // 1. DETECCIÓN DE PISO (Se ejecuta siempre)
        DetectarSuelo();

        // 2. PARÁLISIS (Si está congelado, resetea la carga y no lee teclas)
        if (paralizado)
        {
            ResetearCargaSalto();
            // Mandamos velocidad 0 al Animator para que se quede quieto
            if (anim != null)
            {
                anim.SetFloat("XSpeed", 0f);
                anim.SetFloat("YSpeed", 0f);
            }
            return; 
        }

        // 3. LECTURA DE TECLADO Y ANIMADOR
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");

        // Le enviamos los inputs limpios al Animator Tree (Respetando mayúsculas)
        if (anim != null)
        {
            anim.SetFloat("XSpeed", inputH);
            anim.SetFloat("YSpeed", inputV);
        }

        // 4. SALTO INSTANTÁNEO (Espacio)
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSaltoMaxima * 0.5f, ForceMode.Impulse);
            
            if (anim != null) anim.SetTrigger("IsGrounded");

            if (modoCargar)
            {
                ResetearCargaSalto();
                saltoCargadoCancelado = true;
            }
        }

        // 5. LÓGICA DE SALTO CARGADO (Click Izquierdo)
        ProcesarSaltoCargado();
    }

    void FixedUpdate()
    {
        if (paralizado) return;

        // FÍSICAS DE MOVIMIENTO (Se aplican acá basándose en los inputs del Update)
        Vector3 direccion = (transform.right * inputH + transform.forward * inputV).normalized;

        if (modoCargar && tocaPiso)
        {
            if (direccion == Vector3.zero)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);
                LimitarVelocidad();
            }
        }
        else
        {
            rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);
            LimitarVelocidad();
        }
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================

    private void DetectarSuelo()
    {
        Vector3 origen = transform.position + Vector3.down * offsetSuelo;
        Collider[] colliders = Physics.OverlapSphere(origen, radioDeteccion, capasPiso, QueryTriggerInteraction.Ignore);
        tocaPiso = false;
        foreach (Collider c in colliders)
        {
            if (c.transform.root != transform.root) 
            {
                tocaPiso = true;
                break;
            }
        }
    }

    private void ProcesarSaltoCargado()
    {
        if (Input.GetMouseButtonDown(0) && tocaPiso)
        {
            clickPresionado = true;
            tiempoPresionado = 0f;
            modoCargar = false;
            cargaActual = 0f;
            saltoCargadoCancelado = false;
        }

        if (Input.GetMouseButton(0) && clickPresionado)
        {
            if (!tocaPiso)
            {
                ResetearCargaSalto();
            }
            else
            {
                tiempoPresionado += Time.deltaTime;
                if (tiempoPresionado >= tiempoActivacionCarga)
                {
                    modoCargar = true;
                    cargaActual = Mathf.Clamp01(cargaActual + velocidadCarga * Time.deltaTime);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clickPresionado && tocaPiso && modoCargar && !saltoCargadoCancelado)
            {
                float fuerza = Mathf.Lerp(fuerzaSaltoMaxima * 0.5f, fuerzaSaltoMaxima, cargaActual);
                rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
                
                // Ejecutamos la animación de salto también al soltar el salto cargado
                if (anim != null) anim.SetTrigger("IsGrounded");
            }
            ResetearCargaSalto();
        }
    }

    private void LimitarVelocidad()
    {
        Vector3 velHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velHorizontal.magnitude > velocidadMaxima)
        {
            Vector3 velLimitada = velHorizontal.normalized * velocidadMaxima;
            rb.linearVelocity = new Vector3(velLimitada.x, rb.linearVelocity.y, velLimitada.z);
        }
    }

    private void ResetearCargaSalto()
    {
        clickPresionado = false;
        modoCargar = false;
        cargaActual = 0f;
        tiempoPresionado = 0f;
        saltoCargadoCancelado = false;
    }

    // ==========================================
    // MÉTODOS DE PARÁLISIS Y COLOR (Intactos)
    // ==========================================

    private void InicializarColores()
    {
        allRenderers = GetComponentsInChildren<Renderer>(true);
        if (allRenderers != null && allRenderers.Length > 0)
        {
            meshRenderer = allRenderers[0];
            originalColors = new Color[allRenderers.Length][];
            originalEmissionColors = new Color[allRenderers.Length][];
            originalEmissionEnabled = new bool[allRenderers.Length][];
            
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; 
                originalColors[i] = new Color[mats.Length];
                originalEmissionColors[i] = new Color[mats.Length];
                originalEmissionEnabled[i] = new bool[mats.Length];
                
                for (int j = 0; j < mats.Length; j++)
                {
                    var m = mats[j];
                    if (m == null) { originalColors[i][j] = Color.white; continue; }
                    
                    if (m.HasProperty("_BaseColor")) originalColors[i][j] = m.GetColor("_BaseColor");
                    else if (m.HasProperty("_Color")) originalColors[i][j] = m.GetColor("_Color");
                    else originalColors[i][j] = m.color;
                    
                    if (m.HasProperty("_EmissionColor"))
                    {
                        originalEmissionColors[i][j] = m.GetColor("_EmissionColor");
                        originalEmissionEnabled[i][j] = m.IsKeywordEnabled("_EMISSION");
                    }
                }
            }
            if (originalColors.Length > 0 && originalColors[0].Length > 0)
                colorOriginal = originalColors[0][0];
        }
    }

    void OnGUI()
    {
        if (!modoCargar || !tocaPiso || paralizado) return;

        float anchoFondo = 264f;
        float altoFondo  = 15f;
        float borde      = 2f;

        float x = (Screen.width  - anchoFondo) * 0.5f;
        float y = Screen.height * (1f - 1f / 5f) - altoFondo;

        Rect rectFondo = new Rect(x, y, anchoFondo, altoFondo);
        float anchoBarra = (anchoFondo - borde * 2f) * cargaActual;

        GUI.color = new Color(0f, 0f, 0f, 0.85f);
        GUI.DrawTexture(rectFondo, Texture2D.whiteTexture);

        GUI.color = Color.Lerp(Color.green, Color.red, cargaActual);
        GUI.DrawTexture(new Rect(x + borde, y + borde, anchoBarra, altoFondo - borde * 2f), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    public void Paralizar(float tiempo)
    {
        StopAllCoroutines(); 
        StartCoroutine(RutinaParalisis(tiempo));
    }

    public void ParalizarConEfecto(float tiempo)
    {
        StopAllCoroutines(); 
        StartCoroutine(RutinaParalisiscConEfecto(tiempo));
    }

    private IEnumerator RutinaParalisis(float tiempo)
    {
        paralizado = true;
        yield return new WaitForSeconds(tiempo);
        paralizado = false;
    }

    private IEnumerator RutinaParalisiscConEfecto(float tiempo)
    {
        paralizado = true;
        Color azul = new Color(0f, 0.6f, 1f, 1f);
        float tintStrength = 0.8f; 
        
        if (allRenderers != null)
        {
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; 
                for (int j = 0; j < mats.Length; j++)
                {
                    var m = mats[j];
                    if (m == null) continue;
                    Color orig = Color.white;
                    if (j < originalColors[i].Length) orig = originalColors[i][j];
                    Color target = Color.Lerp(orig, azul, tintStrength);
                    
                    if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", target);
                    if (m.HasProperty("_Color")) m.SetColor("_Color", target);
                    if (!m.HasProperty("_BaseColor") && !m.HasProperty("_Color")) m.color = target;

                    if (m.HasProperty("_EmissionColor"))
                    {
                        m.EnableKeyword("_EMISSION");
                        m.SetColor("_EmissionColor", target * 0.35f);
                    }
                }
            }
        }

        yield return new WaitForSeconds(tiempo);

        if (allRenderers != null && originalColors != null)
        {
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; 
                for (int j = 0; j < mats.Length && j < originalColors[i].Length; j++)
                {
                    var m = mats[j];
                    if (m == null) continue;
                    Color orig = originalColors[i][j];
                    
                    if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", orig);
                    if (m.HasProperty("_Color")) m.SetColor("_Color", orig);
                    if (!m.HasProperty("_BaseColor") && !m.HasProperty("_Color")) m.color = orig;

                    if (m.HasProperty("_EmissionColor") && originalEmissionColors != null)
                    {
                        Color eorig = originalEmissionColors[i][j];
                        m.SetColor("_EmissionColor", eorig);
                        if (originalEmissionEnabled[i][j]) m.EnableKeyword("_EMISSION");
                        else m.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
        paralizado = false;
    }
}