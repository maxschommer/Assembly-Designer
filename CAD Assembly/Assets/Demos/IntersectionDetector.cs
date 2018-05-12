using System.Collections.Generic;
using System.Linq;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using UnityEngine;

public class IntersectionDetector : MonoBehaviour
{
	public GameObject object1;
	public GameObject object2;

	private bool intersectionDetected;
	private World world;
	private RigidBody rigidBody1;
	private RigidBody rigidBody2;

	private void Start()
	{
		world = new World(new CollisionSystemBrute());
		world.CollisionSystem.CollisionDetected += CollisionDetected;

		var mesh1 = object1.GetComponent<MeshFilter>().sharedMesh;
		var shape1 = ConvertMeshToShape(mesh1);
		rigidBody1 = new RigidBody(shape1)
					{
						Position = object1.transform.position.ToJVector(),
						Orientation = object1.transform.rotation.ToJMatrix(),
					};
		world.AddBody(rigidBody1);

		var mesh2 = object2.GetComponent<MeshFilter>().sharedMesh;
		var shape2 = ConvertMeshToShape(mesh2);
		rigidBody2 = new RigidBody(shape2)
					{
						Position = object2.transform.position.ToJVector(),
						Orientation = object2.transform.rotation.ToJMatrix(),
					};
		world.AddBody(rigidBody2);
	}

	private void Update()
	{
		rigidBody1.Position = object1.transform.position.ToJVector();
		rigidBody1.Orientation = object1.transform.rotation.ToJMatrix();

		rigidBody2.Position = object2.transform.position.ToJVector();
		rigidBody2.Orientation = object2.transform.rotation.ToJMatrix();

		DetectIntersection(rigidBody1, rigidBody2);
		Camera.main.backgroundColor = intersectionDetected ? Color.red : Color.green;
	}

	private void CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
	{
		intersectionDetected = true;
	}

	private void DetectIntersection(RigidBody body1, RigidBody body2)
	{
		intersectionDetected = false;
		world.CollisionSystem.Detect(body1, body2);
	}

	private static Shape ConvertMeshToShape(Mesh mesh)
	{
		var vertices = mesh.vertices;
		var vertexList = vertices.Select(p => p.ToJVector()).ToList();

		var indices = mesh.triangles;
		var indexList = new List<TriangleVertexIndices>();
		for (int i = 0; i < indices.Length; i += 3)
		{
			indexList.Add(new TriangleVertexIndices(indices[i + 2], indices[i + 1], indices[i + 0]));
		}

		var octree = new Octree(vertexList, indexList);
		var shape = new TriangleMeshShape(octree);
		return shape;
	}
}