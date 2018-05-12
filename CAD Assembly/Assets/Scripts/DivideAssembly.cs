using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DivideAssembly : MonoBehaviour {
	// Use this for initialization
	void Start()
    {

    }
    public void Initialize(float iconSize)
    {
        if (iconSize == 0)
        {
            iconSize = .05f;
        }
        var basePartScale = 2.5f;

        if (transform.childCount > 0)
        {
            List<Transform> reparent = new List<Transform>(transform.childCount);
            if (transform.GetChild(0).childCount == 0)
            {
                var basePart = new GameObject("basePart");

                basePart.AddComponent<MeshFilter>();
                basePart.GetComponent<MeshFilter>().mesh = gameObject.GetComponent<MeshFilter>().mesh;

                Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

                reparent.Add(basePart.transform);
                Destroy(gameObject.GetComponent<MeshFilter>());

                foreach (Transform child in transform)
                {
                    reparent.Add(child);
                }
            }
            else
            {

                List<Transform> unparent = new List<Transform>(transform.childCount);
                List<GameObject> childObjects = new List<GameObject>(transform.childCount);
                foreach (Transform child in transform)
                {
                    foreach (Transform subChild in child)
                    {
                        reparent.Add(subChild);
                        childObjects.Add(subChild.gameObject);
                    }
                }
                foreach (Transform child in transform)
                {
                    unparent.Add(child);
                    Destroy(child.gameObject);
                }
                foreach (Transform child in unparent)
                {
                    child.parent = null;
                }
            }

            foreach (Transform child in reparent)
            {
                child.parent = transform;
            }

            foreach (Transform child in transform)
            {
                if (child.parent == null)
                {
                    continue;
                }
                child.gameObject.AddComponent<Reference>();
                child.gameObject.AddComponent<MeshCollider>();
                child.gameObject.AddComponent<GrabbedItem>();
                child.gameObject.GetComponent<MeshCollider>().sharedMesh = child.GetComponent<MeshFilter>().mesh;
            }
        }

        float inc = 0;
        foreach (Transform child in transform)
        {
            child.position = new Vector3(0, 0, 0);
            child.rotation = new Quaternion(0, 0, 0, 0);
            Mesh mesh = child.gameObject.GetComponent<MeshFilter>().mesh;
            float maxBounds = Mathf.Max(mesh.bounds.extents.x, mesh.bounds.extents.y, mesh.bounds.extents.z);
            child.localScale = child.localScale * iconSize / (maxBounds) / basePartScale;

            Vector3 Center = child.TransformPoint(mesh.bounds.center);

            child.position = -Center + new Vector3(1f * inc * iconSize, 0, 0);

            inc++;

        }
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.childCount);
        float inc = 0;
        foreach (Transform child in transform)
        {

                
                Mesh mesh = child.gameObject.GetComponent<MeshFilter>().mesh;

                Vector3 worldMeshCenter = child.TransformPoint(mesh.bounds.center);

                child.RotateAround(worldMeshCenter, Vector3.up, 20 * Time.deltaTime);
                inc += 1;
          

        }
    }
}
