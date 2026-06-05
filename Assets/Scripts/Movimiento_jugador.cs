using UnityEngine;
using System.Collections;

public class Movimiento_jugador : MonoBehaviour
{
    private float fuerzaMovimiento = 20f;
    private float velocidadMaxima = 5f;
    private float fuerzaSalto = 5f;

    private Rigidbody rb;
    public bool tocaPiso;
    private bool paralizado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (paralizado)
        {
            return;
        }
        float movHorizontal = Input.GetAxisRaw("Horizontal");
        float movVertical = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (transform.right * movHorizontal + transform.forward * movVertical).normalized;

        rb.AddForce(direccion * fuerzaMovimiento, ForceMode.Force);

        Vector3 velHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velHorizontal.magnitude > velocidadMaxima)
        {
            Vector3 velLimitada = velHorizontal.normalized * velocidadMaxima;
            rb.linearVelocity = new Vector3(velLimitada.x, rb.linearVelocity.y, velLimitada.z);
        }
    }

    void Update()
    {
        if (paralizado)
        {
            return;
        }
        if (Input.GetButtonDown("Jump") && tocaPiso)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            tocaPiso = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
            {
                tocaPiso = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        tocaPiso = false;
    }

    public void Paralizar(float tiempo)
    {
        StartCoroutine(RutinaParalisis(tiempo));
    }

    private IEnumerator RutinaParalisis(float tiempo)
    {
        paralizado = true;

        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(tiempo);

        paralizado = false;
    }
}