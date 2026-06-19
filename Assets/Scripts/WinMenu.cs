using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [Header("UI Elementos")]
    [Tooltip("Arrastra aquí el texto TextMeshPro donde se mostrará quién ganó")]
    public TextMeshProUGUI textoVictoria;
    
    [Tooltip("Arrastra aquí el texto TextMeshPro donde se mostrará el tiempo sobrante")]
    public TextMeshProUGUI textoTiempoSobrante;

    [Tooltip("Arrastra aquí el panel principal del Menú de Victoria")]
    public GameObject panelVictoria;

    [Header("Referencias")]
    public Cronometro cronometro;

    private void Start()
    {
        // Nos aseguramos que el panel empiece desactivado al iniciar la escena
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }

        // Si no asignaron el cronómetro manualmente, intentar buscarlo en la escena
        if (cronometro == null)
        {
            cronometro = FindObjectOfType<Cronometro>();
        }
    }

    public void MostrarMenuVictoria()
    {
        // Obtener el personaje elegido, por defecto "jugador" si no se encuentra
        string personaje = PlayerPrefs.GetString("PersonajeElegido", "jugador");

        if (textoVictoria != null)
        {
            textoVictoria.text = "¡El jugador " + personaje + " gano!";
        }

        // Mostrar tiempo sobrante
        if (textoTiempoSobrante != null && cronometro != null)
        {
            float tiempo = cronometro.ObtenerTiempoRestante();
            int minutos = (int)(tiempo / 60f);
            int segundos = (int)(tiempo % 60f);
            textoTiempoSobrante.text = string.Format("Tiempo sobrante: {0:00}:{1:00}", minutos, segundos);
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
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
