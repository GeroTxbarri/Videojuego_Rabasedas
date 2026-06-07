using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelMenuPrincipal;
    public GameObject panelSeleccionNiveles;
    public GameObject panelSeleccionPersonajes; 
    public GameObject panelConfiguracion;

    // --- FUNCIONES PARA NAVEGAR ENTRE PESTAÑAS ---

    public void AbrirSeleccionNiveles()
    {
        panelMenuPrincipal.SetActive(false); 
        panelSeleccionNiveles.SetActive(true); 
    }

    public void AbrirSeleccionPersonajes()
    {
        panelMenuPrincipal.SetActive(false);
        panelSeleccionPersonajes.SetActive(true);
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

    // --- FUNCIONES DE ACCIÓN ---

    public void JugarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); 
        Application.Quit(); 
    }
}