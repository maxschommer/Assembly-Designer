using UnityEngine;

public class Shooter : MonoBehaviour
{
	public KeyCode keyCode = KeyCode.F;
	public Material material;
	public float lifetime = 10;
	public float mass = 50;
	public float velocity = 10;
	public float angularDrag = 1;
	public float drag = 1;

	private void Update()
	{
		if (Input.GetKeyDown(keyCode) == false)
		{
			return;
		}

		var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.GetComponent<Renderer>().sharedMaterial = material;
		go.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
		go.transform.localScale = .5f * Vector3.one;
		var rb = go.AddComponent<Rigidbody>();
		rb.mass = mass;
		rb.angularDrag = angularDrag;
		rb.drag = drag;
		rb.AddForce(Camera.main.transform.forward * velocity, ForceMode.VelocityChange);

		Destroy(go, lifetime);
	}
}