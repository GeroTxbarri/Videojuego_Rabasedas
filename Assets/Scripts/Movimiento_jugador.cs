using UnityEngine;

public class Movimiento_jugador : MonoBehaviour
{
    private float velocidad = 5f;
    private float fuerzaSalto = 5f;
    private float fuerzaMovimiento = 10f; // Qué tan rápido responde el jugador
    private float velocidadMaxima = 8f;   // Límite de velocidad en suelo normal

    private Rigidbody rb;
    private bool tocaPiso;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (transform.right * movHorizontal + transform.forward * movVertical).normalized;

        // ✅ Aplicamos fuerza en vez de asignar velocidad directamente
        // Así la fricción del Physics Material puede actuar
        rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);

        // Limitamos la velocidad horizontal para que no acelere infinito
        Vector3 velHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velHorizontal.magnitude > velocidadMaxima)
        {
            Vector3 velLimitada = velHorizontal.normalized * velocidadMaxima;
            rb.linearVelocity = new Vector3(velLimitada.x, rb.linearVelocity.y, velLimitada.z);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            tocaPiso = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            tocaPiso = true;
        }
    }
}