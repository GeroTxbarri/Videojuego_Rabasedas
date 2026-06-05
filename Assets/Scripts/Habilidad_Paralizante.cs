using UnityEngine;

using UnityEngine;

public class Habilidad_Paralizante : MonoBehaviour
{
    [Header("Configuración del Paralizante")]
    public KeyCode teclaDisparo = KeyCode.Q;
    public GameObject prefabParalizante;
    public float fuerzaDisparo = 15f;
    public float tiempoParalisis = 3f;

    void Update()
    {
        if (Input.GetKeyDown(teclaDisparo))
        {
            DispararParalizante();
        }
    }

    void DispararParalizante()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direccionDisparo;

        if (h == 0f && v == 0f)
        {
            direccionDisparo = transform.forward;
        }
        else
        {
            direccionDisparo = (transform.right * h + transform.forward * v).normalized;
        }

        Vector3 puntoAparicion = transform.position + direccionDisparo * 1.5f;

        if (prefabParalizante != null)
        {
            GameObject paralizante = Instantiate(prefabParalizante, puntoAparicion, Quaternion.identity);
            Rigidbody rbParalizante = paralizante.GetComponent<Rigidbody>();
            
            if (rbParalizante != null)
            {
                rbParalizante.linearVelocity = GetComponent<Rigidbody>().linearVelocity;
                rbParalizante.AddForce(direccionDisparo * fuerzaDisparo, ForceMode.Impulse);
            }

            Paralizante paralizanteScript = paralizante.GetComponent<Paralizante>();
            if (paralizanteScript != null)
            {
                paralizanteScript.tiempoParalisis = tiempoParalisis;
            }
        }
    }
}