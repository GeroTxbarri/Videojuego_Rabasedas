using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuDerrota : MonoBehaviour
{
    [Header("UI Elementos")]
    [Tooltip("Arrastra aquí el panel principal del Menú de Derrota")]
    public GameObject panelDerrota;

    private void Start()
    {
        // Nos aseguramos que el panel empiece desactivado al iniciar la escena
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }
    }

    public void MostrarMenuDerrota()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
        }

        // Pausar el juego para que no se sigan moviendo
        Time.timeScale = 0f;

        // Desbloquear el cursor para que pueda clickear el botón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolverAlInicio()
    {
        // Reanudar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // Cargar el menú principal
        SceneManager.LoadScene("Menu_Principal");
    }
}
