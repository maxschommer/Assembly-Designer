using Jitter;
using UnityEngine;

public class FpsMeter : MonoBehaviour
{
	public int fps;

	private float previousTime;
	private int frames;

	private void OnGUI()
	{
		GUI.color = Color.magenta;
		GUI.Label(new Rect(0, 0, 100, 50), string.Format("FPS: {0}", fps));
		//GUI.Label(new Rect(0, 30, 200, 50), string.Format("Jitter objects: {0}", JPhysics.World.RigidBodies.Count));
		return;

		float y = 60;
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "PreStep", JPhysics.World.DebugTimes[(int)World.DebugType.PreStep]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "UpdateContacts", JPhysics.World.DebugTimes[(int)World.DebugType.UpdateContacts]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "ClothUpdate", JPhysics.World.DebugTimes[(int)World.DebugType.ClothUpdate]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "CollisionDetect", JPhysics.World.DebugTimes[(int)World.DebugType.CollisionDetect]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "BuildIslands", JPhysics.World.DebugTimes[(int)World.DebugType.BuildIslands]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "DeactivateBodies", JPhysics.World.DebugTimes[(int)World.DebugType.DeactivateBodies]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "IntegrateForces", JPhysics.World.DebugTimes[(int)World.DebugType.IntegrateForces]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "HandleArbiter", JPhysics.World.DebugTimes[(int)World.DebugType.HandleArbiter]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "Integrate", JPhysics.World.DebugTimes[(int)World.DebugType.Integrate]));
		GUI.Label(new Rect(0, y += 25, 200, 50), string.Format("{0}:{1}", "PostStep", JPhysics.World.DebugTimes[(int)World.DebugType.PostStep]));
	}

	private void Update()
	{
		frames++;
		if (Time.time >= previousTime + 1)
		{
			fps = (int)(frames / (Time.time - previousTime));
			previousTime = Time.time;
			frames = 0;
		}
	}
}