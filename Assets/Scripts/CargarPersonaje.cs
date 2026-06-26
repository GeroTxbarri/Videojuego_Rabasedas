using UnityEngine;

public class CargarPersonaje : MonoBehaviour
{
    [Header("Modelos 3D Hijos")]
    public GameObject modeloMilitar;
    public GameObject modeloPez;
    public GameObject modeloRobot;

    [Header("Script de Habilidades de la cápsula")]
    public Habilidad_jugador scriptHabilidades;

    // Usamos Awake en vez de Start. Awake ocurre UN MILISEGUNDO ANTES que Start.
    // Esto es VITAL para prender el modelo antes de que Movimiento_jugador busque el Animator.
    void Awake() 
    {
        // 1. Apagamos todos por precaución
        if (modeloMilitar != null) modeloMilitar.SetActive(false);
        if (modeloPez != null) modeloPez.SetActive(false);
        if (modeloRobot != null) modeloRobot.SetActive(false);

        // 2. Leemos la memoria que guardó tu MenuManager
        // (Asegurate de que las palabras coincidan exacto con lo que pusiste en los botones del menú)
        string personajeElegido = PlayerPrefs.GetString("PersonajeElegido", "Militar");

        // 3. Prendemos el modelo correcto y le seteamos la habilidad "casteando" el número
        switch (personajeElegido)
        {
            case "Militar":
                if (modeloMilitar != null) modeloMilitar.SetActive(true);
                if (scriptHabilidades != null) scriptHabilidades.habilidad = (Habilidad_jugador.TipoHabilidad)2; 
                break;
                
            case "Pez":
                if (modeloPez != null) modeloPez.SetActive(true);
                if (scriptHabilidades != null) scriptHabilidades.habilidad = (Habilidad_jugador.TipoHabilidad)3;
                break;
                
            case "Robot":
                if (modeloRobot != null) modeloRobot.SetActive(true);
                if (scriptHabilidades != null) scriptHabilidades.habilidad = (Habilidad_jugador.TipoHabilidad)1;
                break;
                
            default:
                if (modeloMilitar != null) modeloMilitar.SetActive(true);
                if (scriptHabilidades != null) scriptHabilidades.habilidad = (Habilidad_jugador.TipoHabilidad)2;
                break;
        }
    }
}