using UnityEngine;
using System.Collections;

public class Movimiento_jugador : MonoBehaviour
{
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

    private Rigidbody rb;
    public bool tocaPiso; // calculado por CheckSphere cada frame
    private bool paralizado = false;

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
        rb  = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (paralizado) return;

        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical   = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (transform.right * movHorizontal + transform.forward * movVertical).normalized;

        // Durante la carga: si el jugador NO presiona WASD, anular velocidad horizontal
        // para que el salto sea vertical. Si presiona WASD, se aplica esa dirección.
        if (modoCargar && tocaPiso)
        {
            if (direccion == Vector3.zero)
            {
                // Sin input → forzar quieto horizontalmente
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                // Con WASD → movimiento normal para poder saltar diagonal
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
        if (paralizado) return;

        // ── Detección de piso con esfera ──────────────────────────────────
        Vector3 origen = transform.position + Vector3.down * offsetSuelo;
        Collider[] colliders = Physics.OverlapSphere(origen, radioDeteccion, capasPiso, QueryTriggerInteraction.Ignore);
        tocaPiso = false;
        foreach (Collider c in colliders)
        {
            if (c.transform.root != transform.root) // Ignorar colliders del propio jugador
            {
                tocaPiso = true;
                break;
            }
        }

        // ── Salto instantáneo con Espacio ─────────────────────────────────
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSaltoMaxima * 0.5f, ForceMode.Impulse);

            // Si estaba cargando, cancelar el salto cargado
            if (modoCargar)
            {
                saltoCargadoCancelado = true;
                clickPresionado  = false;
                modoCargar       = false;
                cargaActual      = 0f;
                tiempoPresionado = 0f;
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
                // Salió del piso mientras cargaba → cancelar todo
                clickPresionado  = false;
                modoCargar       = false;
                cargaActual      = 0f;
                tiempoPresionado = 0f;
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

            clickPresionado      = false;
            modoCargar           = false;
            cargaActual          = 0f;
            tiempoPresionado     = 0f;
            saltoCargadoCancelado = false;
        }
    }

    void OnGUI()
    {
        if (!modoCargar || !tocaPiso) return;

        float anchoFondo = 264f;
        float altoFondo  = 15f;
        float borde      = 2f;

        float x = (Screen.width  - anchoFondo) * 0.5f;
        float y = Screen.height * (1f - 1f / 5f) - altoFondo;

        Rect rectFondo = new Rect(x, y, anchoFondo, altoFondo);
        float anchoBarra = (anchoFondo - borde * 2f) * cargaActual;

        // Fondo negro
        GUI.color = new Color(0f, 0f, 0f, 0.85f);
        GUI.DrawTexture(rectFondo, Texture2D.whiteTexture);

        // Barra verde → rojo
        GUI.color = Color.Lerp(Color.green, Color.red, cargaActual);
        GUI.DrawTexture(new Rect(x + borde, y + borde, anchoBarra, altoFondo - borde * 2f), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    public void Paralizar(float tiempo)
    {
        StartCoroutine(RutinaParalisis(tiempo));
    }

    private IEnumerator RutinaParalisis(float tiempo)
    {
        paralizado = true;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(tiempo);
        paralizado = false;
    }
}