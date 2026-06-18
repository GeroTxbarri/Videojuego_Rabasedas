using UnityEngine;
using UnityEngine.UI; // IMPORTANTÍSIMO: Necesario para usar Sliders

public class MenuAjustes : MonoBehaviour
{
    [Header("Elementos de UI")]
    public Slider sliderSensibilidad;

    [Header("Valores por Defecto")]
    public float sensibilidadPorDefecto = 2f; // El valor que tendrá la primera vez que juegues

    void Start()
    {
        // 1. Buscamos si ya hay una sensibilidad guardada de antes. Si no hay, usa la por defecto.
        float sensibilidadGuardada = PlayerPrefs.GetFloat("Sensibilidad", sensibilidadPorDefecto);
        
        // 2. Movemos la "bolita" del slider a esa posición para que coincida visualmente
        if(sliderSensibilidad != null)
        {
            sliderSensibilidad.value = sensibilidadGuardada;
        }
    }

    // Esta función la va a ejecutar el Slider automáticamente cada vez que lo arrastres
    public void GuardarSensibilidad(float nuevoValor)
    {
        // Guardamos el número exacto en la memoria de la compu
        PlayerPrefs.SetFloat("Sensibilidad", nuevoValor);
        PlayerPrefs.Save();
        
        // Opcional: Podés poner un Debug.Log para ver en la consola cómo cambia el número
        // Debug.Log("Nueva sensibilidad guardada: " + nuevoValor);
    }
}