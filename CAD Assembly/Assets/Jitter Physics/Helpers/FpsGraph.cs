using System;
using UnityEngine;

internal class FpsGraph : MonoBehaviour
{
	public bool showFpsGraph;

	private Texture2D texture;
	private int column;
	private new GUITexture guiTexture;

	private void Start()
	{
		texture = CreateTexture();

		guiTexture = gameObject.AddComponent<GUITexture>();
		guiTexture.texture = texture;
		guiTexture.enabled = showFpsGraph;
	}

	private void Update()
	{
		guiTexture.enabled = showFpsGraph;
		if (showFpsGraph)
			UpdateFpsGraph();
	}

	private void OnApplicationQuit()
	{
		Destroy(texture);
	}

	private Texture2D CreateTexture()
	{
		var tex = new Texture2D(1024, 256, TextureFormat.RGBA32, false);
		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
				tex.SetPixel(x, y, default(Color));
		}
		tex.filterMode = FilterMode.Point;

		return tex;
	}

	private void UpdateFpsGraph()
	{
		guiTexture.pixelInset = new Rect(Screen.width / 2f, Screen.height / 2f, 0, 2 * texture.height - Screen.height);

		int height = texture.height;

		const int K = 2500;
		int level = (int)Math.Min(height, Time.deltaTime * K);
		float fps = 1 / Time.deltaTime;

		Color color;
		if (fps > 40)
			color = new Color(0, 1, 0, .5f);
		else if (fps > 20)
			color = new Color(1, 1, 0, .5f);
		else if (fps > 10)
			color = new Color(1, .5f, 0, .5f);
		else
			color = new Color(1, 0, 0, .5f);

		for (int y = 0; y <= level; y++)
			texture.SetPixel(column, y, color);
		for (int y = level + 1; y < height; y++)
			texture.SetPixel(column, y, new Color(0, 0, 0, 0));

		for (int y = 0; y < height; y++)
			texture.SetPixel(column + 1, y, new Color(0, 0, 0, 0));

		for (int i = 10; i <= 60; i += 10)
			texture.SetPixel(column, K / i, new Color(1, 1, 1, .5f));

		texture.Apply(false);
		column++;
		column %= (texture.width - 1);
	}
}