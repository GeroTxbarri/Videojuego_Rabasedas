using UnityEngine;

public class Habilidad_Misil : MonoBehaviour
{
    [Header("Configuración del Disparo")]
    public KeyCode teclaDisparo = KeyCode.E;
    public GameObject premisil;
    public float fuerzaDisparo = 20f;

    void Update()
    {
        if (Input.GetKeyDown(teclaDisparo))
        {
            Disparar();
        }
    }
    void Disparar()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direccionDisparo;

        if (h == 0f && v == 0f) {
            direccionDisparo = transform.forward;
        } 
        else 
        {
            direccionDisparo = (transform.right * h + transform.forward * v).normalized;
        }
        Vector3 puntoAparicion = transform.position + direccionDisparo * 1.5f;

        if (premisil != null)
        {
            GameObject misil = Instantiate(premisil, puntoAparicion, Quaternion.identity);
            Rigidbody rbMisil = misil.GetComponent<Rigidbody>();
            
            if (rbMisil != null)
            {
                rbMisil.AddForce(direccionDisparo * fuerzaDisparo, ForceMode.Impulse);
            }
        }
    }
}