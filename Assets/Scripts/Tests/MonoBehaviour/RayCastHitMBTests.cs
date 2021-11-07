using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RayCastHitMBTests : TestCollection
{
	private class MockRayMB : BaseRayProviderMB
	{
		public Ray ray;

		public override Ray Ray => this.ray;
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectInitAfterStart() {
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHit);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectDefaultNull() {
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		Assert.Null(rayCastHitMB.onHit);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitGameObject() {
		var hit = default(RaycastHit);
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHit!.AddListener(o => hit = o);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreSame(sphere.transform, hit.transform);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectLayer() {
		var hit = default(RaycastHit);
		var sphereDefault = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var cubeWater = GameObject.CreatePrimitive(PrimitiveType.Cube);
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphereDefault.transform.position = Vector3.up;
		cubeWater.layer = LayerMask.NameToLayer("Water");
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;
		rayCastHitMB.layerConstraints = LayerMask.GetMask("Water");

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHit!.AddListener(o => hit = o);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreSame(cubeWater.transform, hit.transform);
	}
}
