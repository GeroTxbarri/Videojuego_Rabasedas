using UnityEngine;
using UnityEngine.UI; // IMPORTANTÍSIMO: Necesario para usar Sliders

public class MenuAjustes : MonoBehaviour
{
    [Header("Elementos de UI")]
    public Slider sliderSensibilidad;
    public Slider sliderVolumen;

    [Header("Valores por Defecto")]
    public float sensibilidadPorDefecto = 2f; // El valor que tendrá la primera vez que juegues
    public float volumenPorDefecto = 1f;

    void Start()
    {
        // 1. Buscamos si ya hay una sensibilidad guardada de antes. Si no hay, usa la por defecto.
        float sensibilidadGuardada = PlayerPrefs.GetFloat("Sensibilidad", sensibilidadPorDefecto);
        
        // 2. Movemos la "bolita" del slider a esa posición para que coincida visualmente
        if(sliderSensibilidad != null)
        {
            sliderSensibilidad.value = sensibilidadGuardada;
        }

        // 3. Lógica para el volumen de la música
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", volumenPorDefecto);
        if(sliderVolumen != null)
        {
            sliderVolumen.value = volumenGuardado;
        }
    }

    // Esta función la va a ejecutar el Slider automáticamente cada vez que lo arrastres
    public void GuardarSensibilidad(float nuevoValor)
    {
        // Guardamos el número exacto en la memoria de la compu
        PlayerPrefs.SetFloat("Sensibilidad", nuevoValor);
        PlayerPrefs.Save();
    }

    // Esta función la va a ejecutar el Slider de volumen automáticamente cada vez que lo arrastres
    public void GuardarVolumen(float nuevoValor)
    {
        PlayerPrefs.SetFloat("VolumenMusica", nuevoValor);
        PlayerPrefs.Save();
        
        // Si la música está sonando, le avisamos que cambie el volumen en tiempo real
        if (ControladorMusica.Instancia != null)
        {
            ControladorMusica.Instancia.ActualizarVolumen(nuevoValor);
        }
    }
}