using UnityEngine;

public class Meta : MonoBehaviour
{
    private bool metaAlcanzada = false;

    [Header("Referencias")]
    public WinMenu winMenu;

    public bool MetaAlcanzada => metaAlcanzada;

    private void OnCollisionEnter(Collision collision)
    {
        // Detectar si el jugador toca la meta
        if (EsJugador(collision.gameObject))
        {
            AlcanzarMeta();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Detectar si el jugador toca la meta (si es Trigger)
        if (EsJugador(collision.gameObject))
        {
            AlcanzarMeta();
        }
    }

    private bool EsJugador(GameObject obj)
    {
        // Verificar si tiene el script de movimiento del jugador o está etiquetado como Player
        return obj.CompareTag("Player") || obj.GetComponent<Movimiento_jugador>() != null;
    }

    private void AlcanzarMeta()
    {
        // Evitar múltiples activaciones
        if (metaAlcanzada)
            return;

        metaAlcanzada = true;

        // Mensaje en consola
        Debug.Log("¡¡¡META ALCANZADA!!!", gameObject);

        // Mostrar el menú de victoria
        if (winMenu != null)
        {
            winMenu.MostrarMenuVictoria();
        }

        // Opcional: hacer algo más (destruir, desactivar, cambiar color, etc.)
        // GetComponent<Renderer>().material.color = Color.green;
    }

    // Método para resetear la meta si es necesario
    public void ResetearMeta()
    {
        metaAlcanzada = false;
    }
}
