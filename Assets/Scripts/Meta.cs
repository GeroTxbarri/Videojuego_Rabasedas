using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Necesario si usas TextMeshPro, si usas Text de Unity cambia a UnityEngine.UI

public class Meta : MonoBehaviour
{
    [Header("UI y Efectos")]
    [Tooltip("El objeto de texto que dice 'Ganaste'")]
    public GameObject textoGanaste;
    
    [Tooltip("El sistema de partículas del confeti")]
    public ParticleSystem confeti;

    private bool metaAlcanzada = false;

    public bool MetaAlcanzada => metaAlcanzada;

    private void Start()
    {
        // Nos aseguramos de que el texto de ganar empiece desactivado
        if (textoGanaste != null)
        {
            textoGanaste.SetActive(false);
        }
    }

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

        // 1. Mostrar texto "Ganaste"
        if (textoGanaste != null)
        {
            textoGanaste.SetActive(true);
        }

        // 2. Hacer caer confeti
        if (confeti != null)
        {
            confeti.Play();
        }

        // 3. Esperar 5 segundos y volver al menú
        StartCoroutine(VolverAlMenu(5f));
    }

    private IEnumerator VolverAlMenu(float tiempoEspera)
    {
        // Esperamos los segundos indicados
        yield return new WaitForSeconds(tiempoEspera);
        
        // Cargamos la escena del menú principal (Hardcodeado)
        SceneManager.LoadScene("Menu_Principal");
    }

    // Método para resetear la meta si es necesario
    public void ResetearMeta()
    {
        metaAlcanzada = false;
        if (textoGanaste != null) textoGanaste.SetActive(false);
    }
}
