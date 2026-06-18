using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPausa;
    public GameObject panelConfiguracion; // Si lo tenés

    // Variable para saber si el juego está pausado o no
    private bool juegoPausado = false;

    void Update()
    {
        // Al tocar la tecla Escape, prendemos o apagamos la pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f; // Congela el tiempo y las físicas
        juegoPausado = true;

        // Liberamos el mouse para que puedas hacer clic en los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        panelPausa.SetActive(false);
        if (panelConfiguracion != null) panelConfiguracion.SetActive(false);
        
        Time.timeScale = 1f; // El tiempo vuelve a correr normal
        juegoPausado = false;

        // Volvemos a ocultar el mouse (ideal para juegos en 3ra persona)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AbrirConfiguracion()
    {
        panelPausa.SetActive(false);
        if (panelConfiguracion != null) panelConfiguracion.SetActive(true);
    }

    public void CerrarConfiguracion()
    {
        if (panelConfiguracion != null) panelConfiguracion.SetActive(false);
        panelPausa.SetActive(true);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; // IMPORTANTÍSIMO: Descongelar el tiempo antes de irse
        SceneManager.LoadScene("Menu_Principal"); // Poné el nombre de tu escena del Menú Principal
    }
}