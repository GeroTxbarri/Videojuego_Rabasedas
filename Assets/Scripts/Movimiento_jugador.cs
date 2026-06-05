using UnityEngine;
using System.Collections;

public class Movimiento_jugador : MonoBehaviour
{
    private float fuerzaMovimiento = 20f;
    private float velocidadMaxima = 5f;
    private float fuerzaSalto = 5f;

    [Header("Salto Cargado")]
    [Tooltip("Fuerza máxima al cargar el salto al 100%")]
    public float fuerzaSaltoMaxima = 15f;
    [Tooltip("Tiempo en segundos para llegar al 100% de carga")]
    public float tiempoCargaMaxima = 2f;
    [Tooltip("Tiempo mínimo presionando espacio para activar el modo carga (seg)")]
    public float tiempoActivacionCarga = 0.3f;

    private Rigidbody rb;
    public bool tocaPiso;
    private bool paralizado = false;

    // --- Lógica de carga ---
    private bool espacioPresionado = false;
    private float tiempoPresionado = 0f;
    private bool modoCargar = false;
    private float cargaActual = 0f; // 0 a 1

    // --- UI barra de carga ---
    private Rect rectBarra;
    private Rect rectFondo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Posición de la barra: abajo a la izquierda, un poco más arriba del borde
        float anchoFondo = 220f;
        float altoFondo = 30f;
        float margenIzq = 30f;
        float margenAbajo = 100f;
        float x = margenIzq;
        float y = Screen.height - margenAbajo - altoFondo;
        rectFondo = new Rect(x, y, anchoFondo, altoFondo);
        rectBarra = new Rect(x + 4, y + 4, 0, altoFondo - 8); // ancho se calcula dinámico
    }

    void FixedUpdate()
    {
        if (paralizado) return;

        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical = Input.GetAxisRaw("Vertical");

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

        // --- Manejo de espacio presionado ---
        if (Input.GetButtonDown("Jump"))
        {
            if (tocaPiso)
            {
                espacioPresionado = true;
                tiempoPresionado = 0f;
                modoCargar = false;
                cargaActual = 0f;
            }
        }

        if (Input.GetButton("Jump") && espacioPresionado && tocaPiso)
        {
            tiempoPresionado += Time.deltaTime;

            // Activar modo carga luego de tiempoActivacionCarga segundos
            if (tiempoPresionado >= tiempoActivacionCarga)
            {
                modoCargar = true;
                // Carga normalizada entre 0 y 1
                float tiempoCargar = tiempoPresionado - tiempoActivacionCarga;
                cargaActual = Mathf.Clamp01(tiempoCargar / tiempoCargaMaxima);
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (espacioPresionado && tocaPiso)
            {
                if (!modoCargar)
                {
                    // Toque rápido → salto normal
                    rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
                }
                else
                {
                    // Salto cargado: interpola entre fuerzaSalto y fuerzaSaltoMaxima
                    float fuerza = Mathf.Lerp(fuerzaSalto, fuerzaSaltoMaxima, cargaActual);
                    rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
                }

                tocaPiso = false;
            }

            // Reset
            espacioPresionado = false;
            modoCargar = false;
            cargaActual = 0f;
            tiempoPresionado = 0f;
        }
    }

    void OnGUI()
    {
        // Solo mostrar barra cuando está cargando
        if (!modoCargar || !tocaPiso) return;

        float anchoMaxBarra = rectFondo.width - 8;
        float anchoBarra = anchoMaxBarra * cargaActual;

        // Fondo oscuro con borde
        GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        GUI.DrawTexture(rectFondo, Texture2D.whiteTexture);

        // Barra de carga: color verde → amarillo → rojo según carga
        Color colorBarra = Color.Lerp(Color.green, Color.red, cargaActual);
        GUI.color = colorBarra;
        GUI.DrawTexture(new Rect(rectFondo.x + 4, rectFondo.y + 4, anchoBarra, rectFondo.height - 8), Texture2D.whiteTexture);

        // Etiqueta
        GUI.color = Color.white;
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 11;
        estilo.alignment = TextAnchor.MiddleCenter;
        estilo.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(rectFondo.x, rectFondo.y - 20, rectFondo.width, 20), "FUERZA SALTO", estilo);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                tocaPiso = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        tocaPiso = false;
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