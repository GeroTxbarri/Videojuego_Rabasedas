using UnityEngine;

public class Camara_Jugador : MonoBehaviour
{
    public float sensibilidad = 2f;

    [Tooltip("Referencia al Transform del jugador para seguir su rotación Y al moverse")]
    public Transform jugador;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Inicializa el yaw con la rotación actual de la cámara
        rotacionY = transform.eulerAngles.y;
    }

    void LateUpdate()

    {
        
        sensibilidad = PlayerPrefs.GetFloat("Sensibilidad", 2f);

        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -20f, 50f);

        // Acumula el giro del mouse
        rotacionY += mouseX;

        // Si hay referencia al jugador, sincroniza el yaw con su rotación
        // (cuando se mueve con WASD, el personaje gira y la cámara lo sigue)
        if (jugador != null)
        {
            rotacionY = jugador.eulerAngles.y + mouseX;
        }

        transform.rotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
    }
}