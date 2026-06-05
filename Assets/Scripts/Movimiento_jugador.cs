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

    private Rigidbody rb;
    public bool tocaPiso; // solo lectura en Inspector, calculado por raycast
    private bool paralizado = false;

    // --- Lógica de carga (click izquierdo) ---
    private bool clickPresionado = false;
    private float tiempoPresionado = 0f;
    private bool modoCargar = false;
    private float cargaActual = 0f; // 0 a 1

    // --- Constantes de activación ---
    private const float tiempoActivacionCarga = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (paralizado) return;

        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical   = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (transform.right * movHorizontal + transform.forward * movVertical).normalized;
        rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);

        Vector3 velHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velHorizontal.magnitude > velocidadMaxima)
        {
            Vector3 velLimitada = velHorizontal.normalized * velocidadMaxima;
            rb.linearVelocity = new Vector3(velLimitada.x, rb.linearVelocity.y, velLimitada.z);
        }
    }

    void Update()
    {
        if (paralizado) return;

        // ── Detección de piso con esfera (confiable contra múltiples colisionadores) ──
        // Lanza una pequeña esfera desde la base del jugador hacia abajo
        Vector3 origen = transform.position + Vector3.down * (GetComponent<Collider>().bounds.extents.y - 0.05f);
        tocaPiso = Physics.CheckSphere(origen, radioDeteccion, capasPiso, QueryTriggerInteraction.Ignore);

        // ── Salto instantáneo con Espacio ──────────────────────────────────
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSaltoMaxima * 0.5f, ForceMode.Impulse);
        }

        // ── Carga con click izquierdo ──────────────────────────────────────
        if (Input.GetMouseButtonDown(0) && tocaPiso)
        {
            clickPresionado = true;
            tiempoPresionado = 0f;
            modoCargar = false;
            cargaActual = 0f;
        }

        if (Input.GetMouseButton(0) && clickPresionado && tocaPiso)
        {
            tiempoPresionado += Time.deltaTime;

            if (tiempoPresionado >= tiempoActivacionCarga)
            {
                modoCargar = true;
                cargaActual = Mathf.Clamp01(cargaActual + velocidadCarga * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clickPresionado && tocaPiso)
            {
                if (modoCargar)
                {
                    // Salto cargado: la fuerza escala con la carga (0% → mitad, 100% → máximo)
                    float fuerza = Mathf.Lerp(fuerzaSaltoMaxima * 0.5f, fuerzaSaltoMaxima, cargaActual);
                    rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
                    tocaPiso = false;
                }
                else
                {
                    // Click corto → no hay acción de salto (el usuario no llegó al umbral)
                }
            }

            clickPresionado = false;
            modoCargar      = false;
            cargaActual     = 0f;
            tiempoPresionado = 0f;
        }
    }

    void OnGUI()
    {
        if (!modoCargar || !tocaPiso) return;

        // ── Dimensiones ────────────────────────────────────────────────────
        // Ancho original era 220 → +20% = 264
        float anchoFondo = 264f;
        // Alto original era 30 → mitad = 15
        float altoFondo  = 15f;
        // Borde: original 4px → mitad = 2px
        float borde      = 2f;

        // ── Posición: centro horizontal, 1/5 desde abajo ──────────────────
        float x = (Screen.width  - anchoFondo) * 0.5f;
        float y = Screen.height * (1f - 1f / 5f) - altoFondo;

        Rect rectFondo = new Rect(x, y, anchoFondo, altoFondo);

        float anchoMaxBarra = anchoFondo - borde * 2f;
        float anchoBarra    = anchoMaxBarra * cargaActual;

        // Fondo negro semitransparente
        GUI.color = new Color(0f, 0f, 0f, 0.85f);
        GUI.DrawTexture(rectFondo, Texture2D.whiteTexture);

        // Barra: verde → rojo según carga
        Color colorBarra = Color.Lerp(Color.green, Color.red, cargaActual);
        GUI.color = colorBarra;
        GUI.DrawTexture(new Rect(x + borde, y + borde, anchoBarra, altoFondo - borde * 2f), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    // tocaPiso se actualiza por CheckSphere en Update() — no se usan más eventos de colisión para esto.

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