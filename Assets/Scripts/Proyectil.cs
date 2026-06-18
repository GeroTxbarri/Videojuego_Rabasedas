using UnityEngine;
public class Proyectil : MonoBehaviour
{
    public float radioExplosion = 4f;
    public float fuerzaEmpuje = 15f;
    public float tiempoVida = 5f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void OnCollisionEnter(Collision collision)
    {
       
        Collider[] objetosAlcanzados = Physics.OverlapSphere(transform.position, radioExplosion);

        foreach (Collider obj in objetosAlcanzados)
        {
            Rigidbody rbDestino = obj.GetComponent<Rigidbody>();

            
            if (rbDestino != null) {
                rbDestino.AddExplosionForce(fuerzaEmpuje, transform.position, radioExplosion, 1f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}