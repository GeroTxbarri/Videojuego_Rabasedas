using UnityEngine;

public class Habilidad_jugador : MonoBehaviour
{
    public enum TipoHabilidad
    {
        Ninguna,
        CaidaLenta,
    }

    [Header("Configuración")]
    public TipoHabilidad habilidad = TipoHabilidad.Ninguna;

    [Header("Caída Lenta")]
    public float factorCaidaLenta = 0.8f;
    public KeyCode teclaHabilidad = KeyCode.E;

    private Rigidbody rb;
    private Movimiento_jugador movimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movimiento = GetComponent<Movimiento_jugador>();
    }

    void Update()
    {
        switch (habilidad)
        {
            case TipoHabilidad.CaidaLenta:
                ManejarCaidaLenta();
                break;

            case TipoHabilidad.Ninguna:
            default:
                break;
        }
    }

    void ManejarCaidaLenta()
    {
        if (Input.GetKey(teclaHabilidad) && !movimiento.tocaPiso && rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y) * factorCaidaLenta, ForceMode.Acceleration);
        }
    }
}