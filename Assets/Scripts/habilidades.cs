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

    // ─────────────────────────────────────────────────────────────────────
    [Header("Paralizante")]
    [Tooltip("Prefab del proyectil paralizante")]
    public GameObject prefabParalizante;
    [Tooltip("Fuerza de lanzamiento del paralizante")]
    public float fuerzaParalizante = 15f;
    [Tooltip("Tiempo que el objetivo queda paralizado (seg)")]
    public float tiempoParalisis = 3f;

    // ─────────────────────────────────────────────────────────────────────
    private Rigidbody rb;
    private Movimiento_jugador movimiento;
    
    // Variable para guardar el estado del input y usarlo en FixedUpdate
    private bool intentandoCaidaLenta = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movimiento = GetComponent<Movimiento_jugador>();

        if (movimiento == null)
        {
            Debug.LogError("Error: Falta el script 'Movimiento_jugador' en este GameObject.");
        }
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
                if (Input.GetKeyDown(teclaHabilidad))
                    DispararMisil();
                break;

            case TipoHabilidad.Paralizante:
                if (Input.GetKeyDown(teclaHabilidad))
                    DispararParalizante();
                break;

            case TipoHabilidad.Ninguna:
            default:
                break;
        }
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

        Vector3 dir = ObtenerDireccionDisparo();
        Vector3 origen = transform.position + dir * 1.5f;

        GameObject misil = Instantiate(prefabMisil, origen, Quaternion.identity);
        Rigidbody rbMisil = misil.GetComponent<Rigidbody>();
        if (rbMisil != null)
            rbMisil.AddForce(dir * fuerzaMisil, ForceMode.Impulse);
    }

    // ── Paralizante ───────────────────────────────────────────────────────
    void DispararParalizante()
    {
        if (prefabParalizante == null)
        {
            Debug.LogError("Error: Prefab Paralizante no asignado en el Inspector.");
            return;
        }

        Vector3 dir = ObtenerDireccionDisparo();
        Vector3 origen = transform.position + dir * 1.5f;

        GameObject proy = Instantiate(prefabParalizante, origen, Quaternion.identity);

        Rigidbody rbProy = proy.GetComponent<Rigidbody>();
        if (rbProy != null)
        {
            // Nota: Al igual que arriba, usa rb.velocity si estás en Unity 2022 o inferior.
            rbProy.linearVelocity = rb.linearVelocity;
            rbProy.AddForce(dir * fuerzaParalizante, ForceMode.Impulse);
        }

        Paralizante script = proy.GetComponent<Paralizante>();
        if (script != null)
            script.tiempoParalisis = tiempoParalisis;
    }

    // ── Utilidad compartida ───────────────────────────────────────────────
    Vector3 ObtenerDireccionDisparo()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0f && v == 0f)
            return transform.forward;

        return (transform.right * h + transform.forward * v).normalized;
    }
}