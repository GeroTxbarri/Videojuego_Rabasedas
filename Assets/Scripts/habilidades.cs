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
    [Header("Misil - Configuración")]
    [Tooltip("Prefab del misil a instanciar")]
    public GameObject prefabMisil;
    [Tooltip("Fuerza de lanzamiento del misil")]
    public float fuerzaMisil = 20f;
    [Tooltip("Cooldown entre disparos de misil (seg)")]
    public float cooldownMisil = 0.5f;

    [Header("Misil - Componentes del Arma")]
    [Tooltip("El objeto RPG (modelo 3D) que está emparentado al hueso del personaje")]
    public GameObject objetoRPG;
    [Tooltip("Objeto vacío en la punta del cañón del RPG de donde saldrá el misil")]
    public Transform puntoDisparoRPG;

    public float retrasoSalidaMisil = 0.15f;

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
    private Animator anim; // Referencia interna para el control del Blend Tree y disparos
    
    // Variable para guardar el estado del input y usarlo en FixedUpdate
    private bool intentandoCaidaLenta = false;

    // Control de cooldowns
    private float tiempoUltimoDisparoMisil = -999f;
    private float tiempoUltimoDisparoParalizante = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movimiento = GetComponent<Movimiento_jugador>();
        anim = GetComponentInChildren<Animator>();

        // Forzamos a que el arma inicie apagada al cargar el mapa
        if (objetoRPG != null)
        {
            objetoRPG.SetActive(false);
        }

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

        if (habilidad == TipoHabilidad.Misil)
        {
            if (prefabMisil == null)
                Debug.LogError("❌ ERROR: Se seleccionó 'Misil' pero no hay prefab asignado. Arrastra el prefab al campo 'Prefab Misil'.");
            
            if (objetoRPG == null)
                Debug.LogWarning("⚠️ ADVERTENCIA: No asignaste el 'Objeto RPG' en el Inspector. El modelo no se prenderá ni apagará.");
        }

        Debug.Log("=================================");
    }

    void Update()
    {
        switch (habilidad)
        {
            case TipoHabilidad.CaidaLenta:
                intentandoCaidaLenta = Input.GetKey(teclaHabilidad);
                break;

            case TipoHabilidad.Misil:
                // Control automático de visibilidad: Oculta el RPG cuando termina el cooldown del disparo
                if (objetoRPG != null && objetoRPG.activeSelf && Time.time >= tiempoUltimoDisparoMisil + cooldownMisil)
                {
                    objetoRPG.SetActive(false);
                }

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
        if (habilidad == TipoHabilidad.CaidaLenta)
        {
            ManejarCaidaLenta();
        }
    }

    // ── Caída Lenta ───────────────────────────────────────────────────────
    void ManejarCaidaLenta()
    {
        if (intentandoCaidaLenta && !movimiento.tocaPiso && rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y) * factorCaidaLenta, ForceMode.Acceleration);
        }
    }

    // ── Misil (Evolucionado y Sincronizado) ────────────────────────────────
    void DispararMisil()
    {
        if (prefabMisil == null)
        {
            Debug.LogError("Error: Prefab de Misil no asignado en el Inspector.");
            return;
        }

        tiempoUltimoDisparoMisil = Time.time;

        // 1. Hace visible el lanzacohetes en la mano de forma inmediata
        if (objetoRPG != null)
        {
            objetoRPG.SetActive(true);
        }

        // 2. Dispara el trigger de retroceso hacia el Animator
        if (anim != null)
        {
            anim.SetTrigger("Disparar");
        }

        StartCoroutine(RutinaSalidaMisil());
    }
    
    System.Collections.IEnumerator RutinaSalidaMisil(){
        yield return new WaitForSeconds(retrasoSalidaMisil);

        Vector3 dir = transform.forward; 
        
        // Determinamos el punto de origen (Usa el punto de disparo si está configurado, si no, usa un offset frontal)
        Vector3 origen = (puntoDisparoRPG != null) ? puntoDisparoRPG.position : (transform.position + dir * 1.5f);

        // 4. Instancia el proyectil alineado con la orientación frontal del personaje
        GameObject misil = Instantiate(prefabMisil, origen, transform.rotation);
        
        // 5. Impulsa el misil en línea recta con fuerza de impacto
        Rigidbody rbMisil = misil.GetComponent<Rigidbody>();
        if (rbMisil != null)
        {
            rbMisil.AddForce(dir * fuerzaMisil, ForceMode.Impulse);
        }
    }
    


    // ── Paralizante (VERSIÓN PROYECTIL - RESTAURADA AL 100%) ──
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