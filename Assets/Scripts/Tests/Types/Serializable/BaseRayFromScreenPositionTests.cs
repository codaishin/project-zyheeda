using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRayFromScreenPositionTests : TestCollection
{
	private class MockScreenPos : IPosition
	{
		public Vector3 position;
		public Vector3 Position => this.position;
	}

	private class MockRayCamToPos : BaseRayFromScreenPosition<MockScreenPos> { }

	[UnityTest]
	public IEnumerator GetRayDirection() {
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			screenPosition = new MockScreenPos(),
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;

		ray.screenPosition.position = new Vector3(Screen.width / 2, Screen.height / 2);
		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.forward,
			ray.Ray.direction,
			delta: new Vector3(0.005f, 0.005f, 0.005f)
		);
	}

	[UnityTest]
	public IEnumerator GetRayOrigin() {
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			screenPosition = new MockScreenPos(),
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;

		ray.screenPosition.position = new Vector3(Screen.width / 2, Screen.height / 2);
		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.up,
			ray.Ray.origin,
			delta: new Vector3(0.005f, 0.005f, 0.01f)
		);
	}
}
