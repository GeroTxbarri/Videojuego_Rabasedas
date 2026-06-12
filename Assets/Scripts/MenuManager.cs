using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelMenuPrincipal;
    public GameObject panelSeleccionNiveles;
    public GameObject panelSeleccionPersonajes;
    public GameObject panelConfiguracion;

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
    }

    // --- ACCIONES DE JUEGO ---

    // Esta función la van a usar los botones de los personajes
    public void SeleccionarPersonajeYJugar(string nombrePersonaje)
    {
        // Guardamos la elección en la memoria de Unity
        PlayerPrefs.SetString("PersonajeElegido", nombrePersonaje);
        PlayerPrefs.Save();

        // Cargamos el nivel de prueba por defecto
        SceneManager.LoadScene("SampleScene"); 
    }

    // Esta función la van a usar los botones del selector de niveles
    public void CargarNivelEspecifico(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}