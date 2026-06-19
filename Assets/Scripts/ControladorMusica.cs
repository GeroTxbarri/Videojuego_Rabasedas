using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    private static ControladorMusica instancia;

    void Awake()
    {
        // Si ya hay una música reproduciéndose, destruimos esta nueva para que no suenen dos canciones a la vez
        if (instancia != null && instancia != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Si no hay ninguna, esta se convierte en la principal
        instancia = this;
        
        // Hacemos que este objeto (con su AudioSource) no se destruya al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
    }
}
