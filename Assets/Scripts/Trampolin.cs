using UnityEngine;

/// <summary>
/// Script para controlar la física del trampolín.
/// Rebota al jugador solo si choca desde arriba.
/// </summary>
public class Trampolin : MonoBehaviour
{
    [Header("Configuración del Rebote")]
    [Tooltip("Fuerza máxima de rebote (Y)")]
    public float fuerzaMaximaRebote = 25f;

    [Tooltip("Porcentaje de aumento de fuerza en rebotes consecutivos (0.2 = 20%)")]
    public float porcentajeAumento = 0.2f;

    [Tooltip("Porcentaje de disminución cuando X > Y (0.1 = 10%)")]
    public float porcentajeDisminucion = 0.1f;

    [Tooltip("Espesor del trampolín (para detectar colisiones desde arriba)")]
    public float espesorTrampolin = 0.5f;

    [Header("Visualización")]
    [Tooltip("Radio de visualización del trampolín")]
    public float radioVisualizacion = 2f;

    // Variables para rastrear el estado del rebote
    private float fuerzaReboteActual = 0f;

    void Start()
    {
        // Inicializar la fuerza de rebote
        fuerzaReboteActual = fuerzaMaximaRebote;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona es el jugador
        Movimiento_jugador movJugador = collision.gameObject.GetComponent<Movimiento_jugador>();
        if (movJugador == null)
            return;

        Rigidbody rbJugador = collision.gameObject.GetComponent<Rigidbody>();
        if (rbJugador == null)
            return;

        // Verificar que la colisión viene desde arriba
        if (!VieneDesdeLaArriba(collision, rbJugador))
            return;

        Debug.Log("🪂 Jugador chocó con el trampolín desde arriba");

        // Obtener la fuerza de caída actual (magnitud de la velocidad Y negativa)
        float fuerzaCaida = Mathf.Abs(rbJugador.linearVelocity.y);

        // Calcular la nueva fuerza de rebote
        float nuevaFuerza = CalcularFuerzaRebote(fuerzaCaida);

        Debug.Log($"Fuerza de caída: {fuerzaCaida:F2} | Nueva fuerza: {nuevaFuerza:F2} | Fuerza máxima: {fuerzaMaximaRebote:F2}");

        // Aplicar el rebote
        AplicarRebote(rbJugador, nuevaFuerza);

        // Actualizar la fuerza de rebote actual para próximos rebotes
        fuerzaReboteActual = nuevaFuerza;
    }

    /// <summary>
    /// Verifica si la colisión viene desde arriba del trampolín
    /// </summary>
    private bool VieneDesdeLaArriba(Collision collision, Rigidbody rb)
    {
        // El jugador debe estar cayendo (velocidad Y negativa)
        if (rb.linearVelocity.y > -0.1f)
            return false;

        float trampolinTop = transform.position.y + espesorTrampolin / 2f;
        float trampolinBottom = transform.position.y - espesorTrampolin / 2f;

        // Verificar que el punto de contacto está en la parte superior
        foreach (ContactPoint contact in collision.contacts)
        {
            // La normal debe apuntar hacia arriba (positiva en Y)
            if (contact.normal.y > 0.3f)
            {
                // El punto de contacto debe estar cerca del tope del trampolín
                if (contact.point.y >= trampolinBottom && contact.point.y <= trampolinTop + 0.5f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Calcula la nueva fuerza de rebote según las reglas del trampolín
    /// </summary>
    private float CalcularFuerzaRebote(float fuerzaCaida)
    {
        // Si la fuerza de caída es menor que la máxima, aumenta un 20%
        if (fuerzaCaida < fuerzaMaximaRebote)
        {
            float fuerzaConAumento = fuerzaCaida * (1f + porcentajeAumento);
            // No puede superar la fuerza máxima
            return Mathf.Min(fuerzaConAumento, fuerzaMaximaRebote);
        }
        else
        {
            // Si cae con más fuerza que la máxima, disminuye un 10%
            return fuerzaCaida * (1f - porcentajeDisminucion);
        }
    }

    /// <summary>
    /// Aplica el rebote al jugador
    /// </summary>
    private void AplicarRebote(Rigidbody rb, float fuerza)
    {
        // Resetear la velocidad Y para asegurar un rebote consistente
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Aplicar la fuerza hacia arriba
        rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
    }

    // Visualización en el editor
    private void OnDrawGizmosSelected()
    {
        // Dibujar un círculo para visualizar el área del trampolín (superior)
        Gizmos.color = Color.yellow;
        DrawWireCircle(transform.position + Vector3.up * espesorTrampolin / 2f, radioVisualizacion, 32);

        // Dibujar un círculo para la base
        DrawWireCircle(transform.position - Vector3.up * espesorTrampolin / 2f, radioVisualizacion, 32);

        // Línea vertical que muestra el espesor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            transform.position + Vector3.up * espesorTrampolin / 2f,
            transform.position - Vector3.up * espesorTrampolin / 2f
        );
    }

    /// <summary>
    /// Dibuja un círculo de wireframe
    /// </summary>
    private void DrawWireCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 lastPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}
