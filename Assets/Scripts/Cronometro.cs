using UnityEngine;

public class Cronometro : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo inicial en segundos antes de perder")]
    public float tiempoInicial = 60f;

    [Header("Referencias")]
    [Tooltip("Arrastra aquí el script de Menú Derrota")]
    public MenuDerrota menuDerrota;

    private float tiempoRestante;
    private bool cronometroActivo = true;
    private Meta meta;

    void Start()
    {
        // Buscar el objeto Meta en la escena
        meta = FindObjectOfType<Meta>();
        
        // Inicializar el tiempo
        tiempoRestante = tiempoInicial;
    }

    void Update()
    {
        // Si el cronómetro está activo, seguir descontando
        if (cronometroActivo)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;
                cronometroActivo = false;
                
                // Mostrar ventana de perder
                if (menuDerrota != null)
                {
                    menuDerrota.MostrarMenuDerrota();
                }
            }

            // Verificar si la meta fue alcanzada para detener el tiempo
            if (meta != null && meta.MetaAlcanzada)
            {
                cronometroActivo = false;
            }
        }
    }

    void OnGUI()
    {
        // Configurar estilos
        GUIStyle estiloCronometro = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };

        // Color del texto (cambia a rojo si queda poco tiempo)
        if (tiempoRestante <= 10f && tiempoRestante > 0f)
            estiloCronometro.normal.textColor = Color.red;
        else
            estiloCronometro.normal.textColor = Color.white;

        // Convertir segundos a formato MM:SS
        int minutos = (int)(tiempoRestante / 60f);
        int segundos = (int)(tiempoRestante % 60f);
        int milisegundos = (int)((tiempoRestante * 100f) % 100f);

        string tiempoFormato = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, milisegundos);

        // Dibuja el cronómetro en la parte superior central de la pantalla
        float anchoPantalla = Screen.width;
        float alturaPantalla = Screen.height;
        float ancho = 300f;
        float alto = 80f;
        float x = (anchoPantalla - ancho) / 2f;
        float y = 20f;

        // Texto del cronómetro
        GUI.Label(new Rect(x, y, ancho, alto), tiempoFormato, estiloCronometro);
    }

    // Getter para obtener el tiempo restante (usado por el menú de victoria)
    public float ObtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    // Método para resetear el cronómetro
    public void ResetearCronometro()
    {
        tiempoRestante = tiempoInicial;
        cronometroActivo = true;
    }
}
