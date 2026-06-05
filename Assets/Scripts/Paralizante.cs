using UnityEngine;

public class Paralizante : MonoBehaviour
{
    public float tiempoParalisis = 3f;
    public float radioEfecto = 5f;
    public Movimiento_jugador portador; // Referencia al que disparó para no afectarlo

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] objetosAlcanzados = Physics.OverlapSphere(transform.position, radioEfecto);

        int jugadoresParalizados = 0;
        foreach (Collider obj in objetosAlcanzados)
        {
            Movimiento_jugador jugador = obj.GetComponent<Movimiento_jugador>();
            
            if (jugador == null)
            {
                jugador = obj.GetComponentInParent<Movimiento_jugador>();
            }

            // No paralizar al portador (quien disparó la habilidad)
            if (jugador != null && jugador != portador)
            {
                jugador.ParalizarConEfecto(tiempoParalisis);
                jugadoresParalizados++;
            }
        }

        Debug.Log($"¡Proyectil paralizante activado! {jugadoresParalizados} jugador(es) paralizado(s) por {tiempoParalisis} segundos.");
        Destroy(gameObject);
    }
}