using UnityEngine;

public class Movimiento_jugador : MonoBehaviour
{
    private float velocidad = 5f;
    private float fuerzaSalto = 5f;
    private Rigidbody rb;
    private bool tocaPiso;

    void Start(){
        rb = GetComponent<Rigidbody>();

    }
    
    void Update(){
        float movHorizontal = Input.GetAxis("Horizontal");
        float movVertical = Input.GetAxis("Vertical");
        Debug.Log("el teclado dice:" + movHorizontal);

        Vector3 movement = new Vector3 (movHorizontal,0.0f,movVertical);
        transform.Translate(movement * velocidad * Time.deltaTime);


        if (Input.GetButtonDown("Jump") && tocaPiso)                {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            tocaPiso = false;
        }
    }
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Ground")){
            tocaPiso=true;
        }
    }

}

