using UnityEngine;
using System.Collections;


public class Movimiento_jugador : MonoBehaviour
{
    private Animator anim;

    private float fuerzaMovimiento = 20f;
    private float velocidadMaxima = 5f;

    [Header("Salto Cargado")]
    [Tooltip("Fuerza máxima al cargar el salto al 100%")]
    public float fuerzaSaltoMaxima = 15f;
    [Tooltip("Velocidad a la que crece la barra de carga (unidades/seg, 1 = llena en 1 seg)")]
    public float velocidadCarga = 0.5f;

    [Header("Detección de Piso")]
    [Tooltip("Radio de la esfera de detección de suelo (ajustar según el tamaño del jugador)")]
    public float radioDeteccion = 0.3f;
    [Tooltip("Layer del suelo (dejar en 'Everything' si no hay layers específicos)")]
    public LayerMask capasPiso = ~0;
    [Tooltip("Desplazamiento hacia abajo desde el centro del jugador para la esfera de detección")]
    public float offsetSuelo = 1f;

    // (La burbuja ahora la genera `ParalisisBurbuja.cs` desde la habilidad)

    private Rigidbody rb;
    public bool tocaPiso; // calculado por CheckSphere cada frame
    private bool paralizado = false;
    private Color colorOriginal = Color.white;
    private Renderer meshRenderer;
    private Renderer[] allRenderers;
    private Color[][] originalColors;
    private Color[][] originalEmissionColors;
    private bool[][] originalEmissionEnabled;

    // Propiedad pública para que el script de habilidades sepa si estamos congelados
    public bool IsParalizado => paralizado;

    // --- Lógica de carga (click izquierdo) ---
    private bool clickPresionado = false;
    private float tiempoPresionado = 0f;
    private bool modoCargar = false;
    private float cargaActual = 0f; // 0 a 1
    private bool saltoCargadoCancelado = false;

    // --- Constante de activación ---
    private const float tiempoActivacionCarga = 0.3f;

    void Start()
    {
        anim=GetComponentInChildren<Animator>();

        rb  = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // Obtener todos los renderers para permitir cambiar color a jugadores con SkinnedMeshRenderer
        allRenderers = GetComponentsInChildren<Renderer>(true);
        if (allRenderers != null && allRenderers.Length > 0)
        {
            meshRenderer = allRenderers[0];
            // Guardar colores originales por renderer/material
            originalColors = new Color[allRenderers.Length][];
            originalEmissionColors = new Color[allRenderers.Length][];
            originalEmissionEnabled = new bool[allRenderers.Length][];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; // this makes instances so changes won't affect sharedMaterials
                originalColors[i] = new Color[mats.Length];
                originalEmissionColors[i] = new Color[mats.Length];
                originalEmissionEnabled[i] = new bool[mats.Length];
                for (int j = 0; j < mats.Length; j++)
                {
                    var m = mats[j];
                    if (m == null) { originalColors[i][j] = Color.white; continue; }
                    if (m.HasProperty("_BaseColor"))
                        originalColors[i][j] = m.GetColor("_BaseColor");
                    else if (m.HasProperty("_Color"))
                        originalColors[i][j] = m.GetColor("_Color");
                    else
                        originalColors[i][j] = m.color;
                    // store emission state/color
                    if (m.HasProperty("_EmissionColor"))
                    {
                        originalEmissionColors[i][j] = m.GetColor("_EmissionColor");
                        originalEmissionEnabled[i][j] = m.IsKeywordEnabled("_EMISSION");
                    }
                }
            }
            // Set a default from first material
            if (originalColors.Length > 0 && originalColors[0].Length > 0)
                colorOriginal = originalColors[0][0];
        }
    }

    void FixedUpdate()
    {
        if (paralizado) return;

        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical   = Input.GetAxisRaw("Vertical");

        anim.SetFloat("Yspeed",movVertical);
        anim.SetFloat("Xspeed",movHorizontal);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("IsGrounded");
        }

        Vector3 direccion = (transform.right * movHorizontal + transform.forward * movVertical).normalized;

        if (modoCargar && tocaPiso)
        {
            if (direccion == Vector3.zero)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);
                Vector3 velH = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                if (velH.magnitude > velocidadMaxima)
                {
                    Vector3 velLim = velH.normalized * velocidadMaxima;
                    rb.linearVelocity = new Vector3(velLim.x, rb.linearVelocity.y, velLim.z);
                }
            }
        }
        else
        {
            rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);
            Vector3 velHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (velHorizontal.magnitude > velocidadMaxima)
            {
                Vector3 velLimitada = velHorizontal.normalized * velocidadMaxima;
                rb.linearVelocity = new Vector3(velLimitada.x, rb.linearVelocity.y, velLimitada.z);
            }
        }
    }

    void Update()
    {
        
        // 1. La detección de piso SIEMPRE debe ejecutarse, incluso estando paralizado
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

        // 2. Si nos paralizan, reseteamos la carga del salto para evitar bugs ópticos o lógicos
        if (paralizado)
        {
            ResetearCargaSalto();
            return; // Detiene el procesamiento de inputs de movimiento
        }

        // ── Salto instantáneo con Espacio ─────────────────────────────────
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSaltoMaxima * 0.5f, ForceMode.Impulse);

            if (modoCargar)
            {
                ResetearCargaSalto();
                saltoCargadoCancelado = true;
            }
        }

        // ── Inicio de carga: solo si está en piso ─────────────────────────
        if (Input.GetMouseButtonDown(0) && tocaPiso)
        {
            clickPresionado      = true;
            tiempoPresionado     = 0f;
            modoCargar           = false;
            cargaActual          = 0f;
            saltoCargadoCancelado = false;
        }

        // ── Acumulación de carga: cancelar si sale del piso ───────────────
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
                    modoCargar  = true;
                    cargaActual = Mathf.Clamp01(cargaActual + velocidadCarga * Time.deltaTime);
                }
            }
        }

        // ── Soltar click ──────────────────────────────────────────────────
        if (Input.GetMouseButtonUp(0))
        {
            if (clickPresionado && tocaPiso && modoCargar && !saltoCargadoCancelado)
            {
                float fuerza = Mathf.Lerp(fuerzaSaltoMaxima * 0.5f, fuerzaSaltoMaxima, cargaActual);
                rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
            }

            ResetearCargaSalto();
        }
    }

    private void ResetearCargaSalto()
    {
        clickPresionado       = false;
        modoCargar            = false;
        cargaActual           = 0f;
        tiempoPresionado      = 0f;
        saltoCargadoCancelado = false;
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
        // Evita superponer corrutinas si ya estás paralizado
        StopAllCoroutines(); 
        StartCoroutine(RutinaParalisis(tiempo));
    }

    public void ParalizarConEfecto(float tiempo)
    {
        // Evita superponer corrutinas si ya estás paralizado
        StopAllCoroutines(); 
        StartCoroutine(RutinaParalisiscConEfecto(tiempo));
    }

    private IEnumerator RutinaParalisis(float tiempo)
    {
        paralizado = true;
        // NO resetear la velocidad: conservar la inercia inicial mientras el jugador
        // queda paralizado (no podrá controlar el movimiento, pero mantiene velocidad).
        yield return new WaitForSeconds(tiempo);
        paralizado = false;
    }

    private IEnumerator RutinaParalisiscConEfecto(float tiempo)
    {
        paralizado = true;

        // Cambiar a color celeste (para todos los renderers y materiales)
        // Aplicar tint hacia azul independientemente de la textura
        Color azul = new Color(0f, 0.6f, 1f, 1f);
        float tintStrength = 0.8f; // 0..1, cuánto aplicar el azul sobre el color original
        if (allRenderers != null)
        {
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; // instances
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

                    // Emission tint to make effect visible on textured materials
                    if (m.HasProperty("_EmissionColor"))
                    {
                        m.EnableKeyword("_EMISSION");
                        m.SetColor("_EmissionColor", target * 0.35f);
                    }
                }
            }
        }

        yield return new WaitForSeconds(tiempo);

        // Restaurar colores originales y estado de emisión
        if (allRenderers != null && originalColors != null)
        {
            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                var mats = r.materials; // instances
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