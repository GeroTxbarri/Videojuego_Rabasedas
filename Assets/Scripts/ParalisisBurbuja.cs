using UnityEngine;
using System.Collections;

public class ParalisisBurbuja : MonoBehaviour
{
    public float radio = 5f;
    public float duracion = 3f;
    public Movimiento_jugador portador;

    // Intervalo de chequeo para aplicar la parálisis a los que estén dentro
    public float intervaloChequeo = 0.25f;

    void Start()
    {
        // Si hay un portador, la burbuja sigue su posición
        if (portador != null)
        {
            transform.SetParent(portador.transform, true);
            transform.localPosition = Vector3.zero;
        }

        StartCoroutine(RutinaBurbuja());
    }

    private IEnumerator RutinaBurbuja()
    {
        float t = 0f;
        while (t < duracion)
        {
            AplicarParalisisAInRange();
            yield return new WaitForSeconds(intervaloChequeo);
            t += intervaloChequeo;
        }

        // Destruir este objeto (también el visual creado por Habilidad_jugador se destruirá junto con él)
        Destroy(gameObject);
    }

    private void AplicarParalisisAInRange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radio);
        foreach (Collider c in cols)
        {
            Movimiento_jugador otro = c.GetComponent<Movimiento_jugador>();
            if (otro == null)
                otro = c.GetComponentInParent<Movimiento_jugador>();

            if (otro != null && otro != portador)
            {
                // Aplicar la parálisis por la duración completa del efecto en el objetivo
                otro.ParalizarConEfecto(duracion);
            }
        }
    }

    // Gizmo para visualizar el radio en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.25f);
        Gizmos.DrawSphere(transform.position, radio);
    }
}
