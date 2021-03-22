using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRayCastHitMBTests : TestCollection
{
	private class MockRayMB : BaseRayProviderMB
	{
		public Ray ray;

		public override Ray Ray => this.ray;
	}

	private class MockRayCastHitMB : BaseRayCastHitMB<GameObject>
	{
		public override bool Get(RaycastHit hit, out GameObject got) {
			got = hit.transform.gameObject;
			return true;
		}
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectInitAfterStart()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<MockRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHit);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectDefaultNull()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<MockRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		Assert.Null(rayCastHitMB.onHit);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitGameObject()
	{
		var hit = null as GameObject;
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<MockRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHit.AddListener(o => hit = o);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreSame(sphere, hit);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectLayer()
	{
		var hit = null as GameObject;
		var sphereDefault = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var cubeWater = GameObject.CreatePrimitive(PrimitiveType.Cube);
		var rayCastHitMB = new GameObject("hitter").AddComponent<MockRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphereDefault.transform.position = Vector3.up;
		cubeWater.layer = LayerMask.NameToLayer("Water");
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;
		rayCastHitMB.layerConstraints = LayerMask.GetMask("Water");

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHit.AddListener(o => hit = o);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreSame(cubeWater, hit);
	}
}