using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rescaleMesh : MonoBehaviour {
    public float ScaleX = 1.0f;
    public float ScaleY = 1.0f;
    public float ScaleZ = 1.0f;
    public bool RecalculateNormals = false;
    private Vector3[] _baseVertices;
    // Use this for initialization
    void Start () {

        foreach (Transform child in transform)
        {
            foreach (Transform subChild in child)
            {
                var mesh = subChild.gameObject.GetComponent<MeshFilter>().mesh;
                if (_baseVertices == null)
                    _baseVertices = mesh.vertices;
                var vertices = new Vector3[_baseVertices.Length];
                for (var i = 0; i < vertices.Length; i++)
                {
                    var vertex = _baseVertices[i];
                    vertex.x = vertex.x * ScaleX;
                    vertex.y = vertex.y * ScaleY;
                    vertex.z = vertex.z * ScaleZ;
                    vertices[i] = vertex;
                }
                mesh.vertices = vertices;
                if (RecalculateNormals)
                    mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

