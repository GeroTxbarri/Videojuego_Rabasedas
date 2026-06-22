using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelMenuPrincipal;
    public GameObject panelSeleccionNiveles;
    public GameObject panelSeleccionPersonajes;
    public GameObject panelConfiguracion;

    // Variable privada para recordar temporalmente qué nivel se eligió
    private string nivelPendiente = "";

    // --- NAVEGACIÓN ---

    public void AbrirSeleccionPersonajes()
    {
        // Ahora este se activa al tocar "JUGAR"
        panelMenuPrincipal.SetActive(false);
        panelSeleccionPersonajes.SetActive(true);
    }

    public void AbrirSeleccionNiveles()
    {
        panelMenuPrincipal.SetActive(false); 
        panelSeleccionNiveles.SetActive(true); 
    }

    public void AbrirConfiguracion()
    {
        panelMenuPrincipal.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        panelSeleccionNiveles.SetActive(false);
        panelSeleccionPersonajes.SetActive(false);
        panelConfiguracion.SetActive(false);
        panelMenuPrincipal.SetActive(true);

        // Si el jugador vuelve atrás, limpiamos el nivel pendiente por seguridad
        nivelPendiente = "";
    }

    // --- ACCIONES DE JUEGO ---

    // Esta función la van a usar los botones de los personajes
    public void SeleccionarPersonajeYJugar(string nombrePersonaje)
    {
        // Guardamos la elección en la memoria de Unity
        PlayerPrefs.SetString("PersonajeElegido", nombrePersonaje);
        PlayerPrefs.Save();

        // Si la variable tiene un nivel guardado, carga ese nivel específico.
        // Si está vacía (porque quizás tocaron "JUGAR" directo desde el menú principal), 
        // usa "SampleScene" como nivel por defecto.
        if (!string.IsNullOrEmpty(nivelPendiente))
        {
            SceneManager.LoadScene(nivelPendiente);
        }
        else
        {
            SceneManager.LoadScene("SampleScene"); 
        }
    }

    // Esta función la van a usar los botones del selector de niveles
    public void CargarNivelEspecifico(string nombreEscena)
    {
        // 1. Guardamos el nombre de la escena en el "bolsillo"
        nivelPendiente = nombreEscena;

        // 2. En vez de cargar la escena de golpe, hacemos el cambio de paneles
        panelSeleccionNiveles.SetActive(false);
        panelSeleccionPersonajes.SetActive(true);
    }
}