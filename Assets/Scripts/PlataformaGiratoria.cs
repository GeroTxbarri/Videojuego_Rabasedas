using UnityEngine;

public class PlataformaGiratoria : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [SerializeField]
    private Vector3 puntocentral = Vector3.zero;
    
    [SerializeField]
    [Tooltip("Radio de rotación en metros")]
    private float radio = 5f;
    
    [SerializeField]
    [Tooltip("Velocidad de rotación en grados por segundo")]
    private float velocidadRotacion = 45f;

    private float anguloActual = 0f;
    private Transform jugadorActual;
    private Rigidbody rbJugador;
    private Vector3 velocidadTangencialAplicada = Vector3.zero;
    private Vector3 posicionAnterior = Vector3.zero;

    void Start()
    {
        // Posiciona la plataforma en el punto inicial (radio grados de 0)
        ActualizarPosicionPlataforma(0f);
        posicionAnterior = transform.position;
    }

    void FixedUpdate()
    {
        // Guardar la posición anterior de la plataforma
        Vector3 posicionActualPlataforma = transform.position;

        // Actualizar ángulo
        anguloActual += velocidadRotacion * Time.fixedDeltaTime;
        if (anguloActual >= 360f)
            anguloActual -= 360f;

        // Actualizar posición de la plataforma
        ActualizarPosicionPlataforma(anguloActual);

        // Calcular el delta de movimiento de la plataforma
        Vector3 deltaMovimiento = transform.position - posicionActualPlataforma;

        // Si el jugador está sobre la plataforma, moverlo junto con ella
        if (jugadorActual != null && rbJugador != null)
        {
            MoverJugadorConPlataforma(deltaMovimiento);
        }

        posicionAnterior = transform.position;
    }

    /// <summary>
    /// Actualiza la posición de la plataforma basada en el ángulo actual
    /// </summary>
    private void ActualizarPosicionPlataforma(float angulo)
    {
        float radianes = angulo * Mathf.Deg2Rad;
        
        Vector3 nuevaPosicion = puntocentral + new Vector3(
            Mathf.Cos(radianes) * radio,
            0f,
            Mathf.Sin(radianes) * radio
        );

        transform.position = nuevaPosicion;
    }

    /// <summary>
    /// Mueve al jugador junto con la plataforma
    /// </summary>
    private void MoverJugadorConPlataforma(Vector3 deltaMovimiento)
    {
        if (jugadorActual == null || rbJugador == null)
            return;

        // Mover el jugador la misma cantidad que se movió la plataforma
        jugadorActual.position += deltaMovimiento;
    }

    /// <summary>
    /// Detecta cuando el jugador entra en la plataforma
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Buscar un Rigidbody en el objeto colisionado
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        
        // También verificar si tiene el script de movimiento del jugador
        Movimiento_jugador movJugador = collision.gameObject.GetComponent<Movimiento_jugador>();

        // Si tiene Rigidbody y (tiene movimiento de jugador O tiene tag Player)
        if (rb != null && (movJugador != null || collision.gameObject.CompareTag("Player")))
        {
            jugadorActual = collision.transform;
            rbJugador = rb;
        }
    }

    /// <summary>
    /// Detecta cuando el jugador sale de la plataforma
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == jugadorActual)
        {
            jugadorActual = null;
            rbJugador = null;
        }
    }

    #region Propiedades Públicas (para edición en runtime si es necesario)

    public Vector3 PuntoCentral
    {
        get { return puntocentral; }
        set { puntocentral = value; }
    }

    public float Radio
    {
        get { return radio; }
        set { radio = Mathf.Max(0.1f, value); }
    }

    public float VelocidadRotacion
    {
        get { return velocidadRotacion; }
        set { velocidadRotacion = value; }
    }

    #endregion

    #region Dibujar en el Editor

    void OnDrawGizmos()
    {
        // Dibujar el punto central
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(puntocentral, 0.3f);

        // Dibujar la trayectoria circular aproximada
        Gizmos.color = Color.yellow;
        int puntos = 32;
        for (int i = 0; i < puntos; i++)
        {
            float angle1 = (360f / puntos) * i * Mathf.Deg2Rad;
            float angle2 = (360f / puntos) * (i + 1) * Mathf.Deg2Rad;

            Vector3 pos1 = puntocentral + new Vector3(
                Mathf.Cos(angle1) * radio,
                0f,
                Mathf.Sin(angle1) * radio
            );

            Vector3 pos2 = puntocentral + new Vector3(
                Mathf.Cos(angle2) * radio,
                0f,
                Mathf.Sin(angle2) * radio
            );

            Gizmos.DrawLine(pos1, pos2);
        }

        // Mostrar la posición actual de la plataforma
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }

    #endregion
}
