using UnityEngine;

public class Cronometro : MonoBehaviour
{
    private float tiempoTranscurrido = 0f;
    private bool cronometroActivo = true;
    private Meta meta;

    void Start()
    {
        // Buscar el objeto Meta en la escena
        meta = FindObjectOfType<Meta>();
    }

    void Update()
    {
        // Si el cronómetro está activo, seguir contando
        if (cronometroActivo)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Verificar si la meta fue alcanzada
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

        // Color del texto
        estiloCronometro.normal.textColor = Color.white;

        // Convertir segundos a formato MM:SS
        int minutos = (int)(tiempoTranscurrido / 60f);
        int segundos = (int)(tiempoTranscurrido % 60f);
        int milisegundos = (int)((tiempoTranscurrido * 100f) % 100f);

        string tiempoFormato = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, milisegundos);

        // Dibuja el cronómetro en la parte superior central de la pantalla
        float anchoPantalla = Screen.width;
        float alturaPantalla = Screen.height;
        float ancho = 300f;
        float alto = 80f;
        float x = (anchoPantalla - ancho) / 2f;
        float y = 20f;

        // Texto del cronómetro
        GUI.color = Color.white;
        GUI.Label(new Rect(x, y, ancho, alto), tiempoFormato, estiloCronometro);

        GUI.color = Color.white;
    }

    // Getter para obtener el tiempo transcurrido
    public float ObtenerTiempoTranscurrido()
    {
        return tiempoTranscurrido;
    }

    // Método para resetear el cronómetro
    public void ResetearCronometro()
    {
        tiempoTranscurrido = 0f;
        cronometroActivo = true;
    }
}
