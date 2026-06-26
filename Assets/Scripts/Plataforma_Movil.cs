using UnityEngine;

public class PlataformaMovil : MonoBehaviour
{
	public Transform puntoA;
	public Transform puntoB;
	public float velocidad = 2f;

	private Transform objetivo;

	void Start()
	{
		objetivo = puntoB;
	}

	void Update()
	{
		transform.position = Vector3.MoveTowards(
			transform.position,
			objetivo.position,
			velocidad * Time.deltaTime
		);

		if (Vector3.Distance(transform.position, objetivo.position) < 0.05f)
		{
			objetivo = objetivo == puntoA ? puntoB : puntoA;
		}
	}
}