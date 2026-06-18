using UnityEngine;

public class Trampolin : MonoBehaviour
{
    [Header("Configuración del Trampolín")]
    [Tooltip("Fuerza con la que el trampolín impulsa al jugador hacia arriba. Puedes modificarla desde el Inspector.")]
    public float fuerzaRebote = 20f;

    private void OnCollisionEnter(Collision collision)
    {
        // Verificamos si el objeto que colisiona tiene el script del jugador
        Movimiento_jugador jugador = collision.gameObject.GetComponent<Movimiento_jugador>();
        
        if (jugador != null)
        {
            Rigidbody rbJugador = collision.gameObject.GetComponent<Rigidbody>();
            if (rbJugador != null)
            {
                // Reseteamos la velocidad vertical para que el rebote sea siempre consistente
                // independientemente de la velocidad de caída.
                rbJugador.linearVelocity = new Vector3(rbJugador.linearVelocity.x, 0f, rbJugador.linearVelocity.z);
                
                // Aplicamos la fuerza de rebote hacia arriba
                rbJugador.AddForce(Vector3.up * fuerzaRebote, ForceMode.Impulse);
            }
        }
    }
}
