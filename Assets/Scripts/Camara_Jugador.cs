using UnityEngine;

public class Camara_Jugador : MonoBehaviour
{
    public float sensibilidad = 2f; 

    public Transform cuerpoJugador;

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
        
        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        cuerpoJugador.Rotate(Vector3.up * mouseX);
    }
}