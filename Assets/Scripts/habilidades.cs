using UnityEngine;

/// <summary>
/// Componente único de habilidades del jugador.
/// </summary>
[RequireComponent(typeof(Rigidbody))] // Asegura que siempre haya un Rigidbody
public class Habilidad_jugador : MonoBehaviour
{
    public enum TipoHabilidad
    {
        Ninguna,
        CaidaLenta,
        Misil,
        Paralizante,
    }

    // ─────────────────────────────────────────────────────────────────────
    [Header("Habilidad activa")]
    public TipoHabilidad habilidad = TipoHabilidad.Ninguna;
    [Tooltip("Tecla para activar la habilidad seleccionada")]
    public KeyCode teclaHabilidad = KeyCode.E;

    // ─────────────────────────────────────────────────────────────────────
    [Header("Caída Lenta")]
    [Tooltip("Factor con el que se contrarresta la gravedad (0 = sin efecto, 1 = gravedad cero)")]
    public float factorCaidaLenta = 0.8f;

    // ─────────────────────────────────────────────────────────────────────
    [Header("Misil")]
    [Tooltip("Prefab del misil a instanciar")]
    public GameObject prefabMisil;
    [Tooltip("Fuerza de lanzamiento del misil")]
    public float fuerzaMisil = 20f;
    [Tooltip("Cooldown entre disparos de misil (seg)")]
    public float cooldownMisil = 0.5f;

    // ─────────────────────────────────────────────────────────────────────
    [Header("Paralizante")]
    [Tooltip("Prefab del proyectil paralizante (si usas proyectil). No requerido para la burbuja")]
    public GameObject prefabParalizante;
    [Tooltip("Fuerza de lanzamiento del paralizante")]
    public float fuerzaParalizante = 15f;
    [Tooltip("Tiempo que el objetivo queda paralizado (seg)")]
    public float tiempoParalisis = 3f;
    [Tooltip("Cooldown entre disparos paralizantes (seg)")]
    public float cooldownParalizante = 1f;
    [Tooltip("Radio de la burbuja de parálisis (si se usa la burbuja)")]
    public float radioParalizante = 5f;

    // ─────────────────────────────────────────────────────────────────────
    private Rigidbody rb;
    private Movimiento_jugador movimiento;
    
    // Variable para guardar el estado del input y usarlo en FixedUpdate
    private bool intentandoCaidaLenta = false;

    // Control de cooldowns
    private float tiempoUltimoDisparoMisil = -999f;
    private float tiempoUltimoDisparoParalizante = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movimiento = GetComponent<Movimiento_jugador>();

        if (movimiento == null)
        {
            Debug.LogError("Error: Falta el script 'Movimiento_jugador' en este GameObject.");
        }

        // Validaciones de configuración
        ValidarConfiguracion();
    }

    private void ValidarConfiguracion()
    {
        Debug.Log("=== VALIDACIÓN DE HABILIDADES ===");
        Debug.Log($"Habilidad seleccionada: {habilidad}");
        Debug.Log($"Tecla de activación: {teclaHabilidad}");

        if (habilidad == TipoHabilidad.Ninguna)
        {
            Debug.LogWarning("⚠️ ADVERTENCIA: La habilidad está configurada como 'Ninguna'. Asigna una en el Inspector.");
        }

        if (habilidad == TipoHabilidad.Misil && prefabMisil == null)
        {
            Debug.LogError("❌ ERROR: Se seleccionó 'Misil' pero no hay prefab asignado. Arrastra el prefab al campo 'Prefab Misil'.");
        }

        // El paralizante puede usar una burbuja; el prefab del proyectil es opcional.

        Debug.Log("=================================");
    }

    void Update()
    {
        switch (habilidad)
        {
            case TipoHabilidad.CaidaLenta:
                // Solo registramos el input en Update
                intentandoCaidaLenta = Input.GetKey(teclaHabilidad);
                break;

            case TipoHabilidad.Misil:
                if (Input.GetKeyDown(teclaHabilidad) && PuedoDispararMisil())
                    DispararMisil();
                break;

            case TipoHabilidad.Paralizante:
                if (Input.GetKeyDown(teclaHabilidad) && PuedoDispararParalizante())
                    DispararParalizante();
                break;

            case TipoHabilidad.Ninguna:
            default:
                break;
        }
    }

    private bool PuedoDispararMisil()
    {
        return Time.time >= tiempoUltimoDisparoMisil + cooldownMisil;
    }

    private bool PuedoDispararParalizante()
    {
        return Time.time >= tiempoUltimoDisparoParalizante + cooldownParalizante;
    }

    void FixedUpdate()
    {
        // Las fuerzas físicas continuas deben aplicarse aquí
        if (habilidad == TipoHabilidad.CaidaLenta)
        {
            ManejarCaidaLenta();
        }
    }

    // ── Caída Lenta ───────────────────────────────────────────────────────
    void ManejarCaidaLenta()
    {
        // Nota: rb.linearVelocity es exclusivo de Unity 6+. 
        // Si usas una versión anterior y te da error, cámbialo a rb.velocity.y
        if (intentandoCaidaLenta && !movimiento.tocaPiso && rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y) * factorCaidaLenta, ForceMode.Acceleration);
        }
    }

    // ── Misil ─────────────────────────────────────────────────────────────
    void DispararMisil()
    {
        if (prefabMisil == null)
        {
            Debug.LogError("Error: Prefab de Misil no asignado en el Inspector.");
            return;
        }

        tiempoUltimoDisparoMisil = Time.time;

        Vector3 dir = transform.forward; // Dirección hacia donde está mirando el jugador
        Vector3 origen = transform.position + dir * 1.5f;

        GameObject misil = Instantiate(prefabMisil, origen, Quaternion.identity);
        Rigidbody rbMisil = misil.GetComponent<Rigidbody>();
        if (rbMisil != null)
            rbMisil.AddForce(dir * fuerzaMisil, ForceMode.Impulse);
    }

    // ── Paralizante (VERSIÓN PROYECTIL - RESTAURADA) ──
    void DispararParalizante()
    {
        if (prefabParalizante == null)
        {
            Debug.LogError("Error: Prefab de Paralizante no asignado en el Inspector.");
            return;
        }

        tiempoUltimoDisparoParalizante = Time.time;

        // Obtener dirección de disparo (hacia donde mira el jugador)
        Vector3 direccionDisparo = transform.forward;
    
        // También puedes usar input de movimiento como antes:
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
    
        if (h != 0f || v != 0f)
        {
            direccionDisparo = (transform.right * h + transform.forward * v).normalized;
        }
    
        Vector3 puntoAparicion = transform.position + direccionDisparo * 1.5f;
    
        GameObject paralizante = Instantiate(prefabParalizante, puntoAparicion, Quaternion.identity);
    
        // Configurar el proyectil
        Paralizante paralizanteScript = paralizante.GetComponent<Paralizante>();
        if (paralizanteScript != null)
        {
            paralizanteScript.tiempoParalisis = tiempoParalisis;
            paralizanteScript.portador = GetComponent<Movimiento_jugador>();
            paralizanteScript.radioEfecto = radioParalizante; // Usar el radio para el área de efecto
        }
    
        // Agregar fuerza física
        Rigidbody rbParalizante = paralizante.GetComponent<Rigidbody>();
        if (rbParalizante != null)
        {
            rbParalizante.AddForce(direccionDisparo * fuerzaParalizante, ForceMode.Impulse);
        }
    
        Debug.Log($"Disparo paralizante lanzado en dirección: {direccionDisparo}");
    }

}