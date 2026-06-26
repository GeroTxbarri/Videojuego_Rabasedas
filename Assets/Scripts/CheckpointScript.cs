using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [Header("Punto de Respawn")]
    [Tooltip("Arrastrá aquí el Transform vacío que indica dónde reaparecerá el jugador")]
    public Transform puntoRespawn;

    [Header("Visual (Opcional)")]
    [Tooltip("Color del checkpoint cuando está INACTIVO")]
    public Color colorInactivo = new Color(1f, 1f, 0f, 0.3f); // Amarillo semitransparente
    [Tooltip("Color del checkpoint cuando está ACTIVO (ya fue tocado)")]
    public Color colorActivo = new Color(0f, 1f, 0f, 0.3f);   // Verde semitransparente

    private bool activado = false;
    private Renderer vistaCheckpoint;

    private void Start()
    {
        // Si tiene un Renderer (ej: un cubo en la escena), le asignamos el color inactivo
        vistaCheckpoint = GetComponent<Renderer>();
        if (vistaCheckpoint != null)
        {
            vistaCheckpoint.material.color = colorInactivo;
        }
    }

    // Se activa cuando el jugador entra al trigger (si la caja tiene Is Trigger = true)
    private void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player") || otro.GetComponent<Logica_Respawn>() != null)
        {
            ActivarCheckpoint(otro.gameObject);
        }
    }

    private void ActivarCheckpoint(GameObject jugador)
    {
        // Verificamos que el checkpoint tenga un punto de respawn asignado
        if (puntoRespawn == null)
        {
            Debug.LogWarning($"[Checkpoint] '{gameObject.name}' no tiene un Punto de Respawn asignado.", gameObject);
            return;
        }

        // Buscamos el script de respawn en el jugador
        Logica_Respawn logicaRespawn = jugador.GetComponent<Logica_Respawn>();

        if (logicaRespawn == null)
        {
            // El jugador puede ser una cápsula padre; buscamos también en el padre
            logicaRespawn = jugador.GetComponentInParent<Logica_Respawn>();
        }

        if (logicaRespawn != null)
        {
            // Actualizamos el punto de respawn al de este checkpoint
            logicaRespawn.puntoInicio = puntoRespawn;

            // Marcamos este checkpoint como activado
            activado = true;

            // Cambiamos el color si tiene renderer
            if (vistaCheckpoint != null)
            {
                vistaCheckpoint.material.color = colorActivo;
            }

            Debug.Log($"[Checkpoint] '{gameObject.name}' activado. El jugador reaparecerá aquí.", gameObject);
        }
        else
        {
            Debug.LogWarning("[Checkpoint] No se encontró el script 'Logica_Respawn' en el jugador.", jugador);
        }
    }

    // Método para saber si este checkpoint fue activado (útil para UI o lógica futura)
    public bool EstaActivado() => activado;
}
