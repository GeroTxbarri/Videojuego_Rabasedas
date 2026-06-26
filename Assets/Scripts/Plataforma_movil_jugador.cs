using UnityEngine;

public class PlataformaPadre : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		collision.transform.SetParent(transform);
	}

	private void OnCollisionExit(Collision collision)
	{
		collision.transform.SetParent(null);
	}
}