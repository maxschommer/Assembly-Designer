using UnityEngine;

internal class TerrainProbe : MonoBehaviour
{
	private void Update()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit = JPhysics.Raycast(ray);
		if (hit != null)
		{
			if (Input.GetMouseButtonDown(2))
				Dig(hit.Point);
		}
	}

	private void Dig(Vector3 point)
	{
		var jterrain = FindObjectOfType<JTerrainCollider>();
		int resolution = jterrain.Resolution;
		var size = jterrain.Size;

		var origin = jterrain.transform.position;
		int x = Mathf.RoundToInt((point.x - origin.x) / size.x * resolution);
		int z = Mathf.RoundToInt((point.z - origin.z) / size.z * resolution);

		var terrain = jterrain.GetComponent<Terrain>();
		var data = terrain.terrainData;
		float h = data.GetHeight(x, z);
		h = (h + .1f) / size.y;
		data.SetHeights(x, z, new[,] { { h } });

		jterrain.UpdateShape();
	}
}