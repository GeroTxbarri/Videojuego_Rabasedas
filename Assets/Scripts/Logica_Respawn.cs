using UnityEngine;

public class Logica_Respawn : MonoBehaviour
{
    public Transform puntoInicio;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("zonaMuerte"))
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = puntoInicio.position;
        }
    }
}
