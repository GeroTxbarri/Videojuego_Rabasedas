using UnityEngine;

public class Paralizante : MonoBehaviour
{
    public float tiempoParalisis = 3f;
    public float radioEfecto = 5f;
    public Movimiento_jugador portador;

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

            if (jugador != null && jugador != portador)
            {
                jugador.Paralizar(tiempoParalisis);
                jugadoresParalizados++;
            }
        }

        if (jugadoresParalizados > 0)
        {
            Debug.Log($"¡Proyectil paralizante impactó! {jugadoresParalizados} jugador(es) paralizado(s) por {tiempoParalisis} segundos.");
        }
        
        Destroy(gameObject);
    }
}