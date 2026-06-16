using UnityEngine;

public class Camara_Jugador : MonoBehaviour
{
    public float sensibilidad = 2f;

    private float rotacionX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -20f, 50f);

        // Solo rota la camara en pitch (arriba/abajo) y yaw (izquierda/derecha)
        // El cuerpo del jugador ya NO gira con el mouse; lo hace solo al moverse
        transform.rotation = Quaternion.Euler(rotacionX, transform.eulerAngles.y + mouseX, 0f);
    }
}