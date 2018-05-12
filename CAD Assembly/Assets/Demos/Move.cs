using UnityEngine;

public class Move : MonoBehaviour
{
	private float x;

	private void Start()
	{
		x = transform.position.x;
	}

	private void Update()
	{
		var position = transform.position;
		position.x = x + 1f*Mathf.Sin(Time.time);
		transform.position = position;
	}
}