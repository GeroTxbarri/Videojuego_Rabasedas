using UnityEngine;

public class Paralizante : MonoBehaviour
{
    public float tiempoParalisis = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        Movimiento_jugador jugador = collision.gameObject.GetComponent<Movimiento_jugador>();
        
        if (jugador == null)
        {
            jugador = collision.gameObject.GetComponentInParent<Movimiento_jugador>();
        }

        if (jugador != null)
        {
            jugador.Paralizar(tiempoParalisis);
            Debug.Log("¡Jugador paralizado por " + tiempoParalisis + " segundos!");
        }

        Destroy(gameObject);
    }
}