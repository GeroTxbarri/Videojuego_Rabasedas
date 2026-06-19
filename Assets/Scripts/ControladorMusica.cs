using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    public static ControladorMusica Instancia { get; private set; }
    private AudioSource audioSource;

    void Awake()
    {
        // Si ya hay una música reproduciéndose, destruimos esta nueva para que no suenen dos canciones a la vez
        if (Instancia != null && Instancia != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Si no hay ninguna, esta se convierte en la principal
        Instancia = this;
        
        // Obtenemos el AudioSource
        audioSource = GetComponent<AudioSource>();

        // Hacemos que este objeto (con su AudioSource) no se destruya al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // Al iniciar, leemos el volumen guardado y lo aplicamos (por defecto 1.0f que es 100%)
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        }
    }

    // Método para cambiar el volumen en tiempo real desde los ajustes
    public void ActualizarVolumen(float volumen)
    {
        if (audioSource != null)
        {
            audioSource.volume = volumen;
        }
    }
}
