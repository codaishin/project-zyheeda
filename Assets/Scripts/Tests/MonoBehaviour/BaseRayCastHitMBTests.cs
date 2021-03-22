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

	[UnityTest]
	public IEnumerator OnHitGameObjectInitAfterStart()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHitObject);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectDefaultNull()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		Assert.Null(rayCastHitMB.onHitObject);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitGameObject()
	{
		var hit = null as GameObject;
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitObject.AddListener(o => hit = o);

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
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphereDefault.transform.position = Vector3.up;
		cubeWater.layer = LayerMask.NameToLayer("Water");
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;
		rayCastHitMB.layerConstraints = LayerMask.GetMask("Water");

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitObject.AddListener(o => hit = o);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreSame(cubeWater, hit);
	}

	[UnityTest]
	public IEnumerator OnHitVector3InitAfterStart()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHitPoint);
	}

	[UnityTest]
	public IEnumerator OnHitVector3DefaultNull()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();
		rayCastHitMB.raySource = rayProviderMB;

		Assert.Null(rayCastHitMB.onHitPoint);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitVector3()
	{
		var hit = Vector3.zero;
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitPoint.AddListener(v => hit = v);

		yield return new WaitForSeconds(0.5f); // FIXME: Sometimes odd behaviour when not waiting

		rayCastHitMB.TryHit();

		Assert.AreEqual(new Vector3(0, 1.5f, 0), hit);
	}

	[UnityTest]
	public IEnumerator OnHitVector3Layer()
	{
		var hit = Vector3.zero;
		var sphereDefault = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var cubeWater = GameObject.CreatePrimitive(PrimitiveType.Cube);
		var rayCastHitMB = new GameObject("hitter").AddComponent<BaseRayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphereDefault.transform.position = Vector3.up;
		cubeWater.layer = LayerMask.NameToLayer("Water");
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.raySource = rayProviderMB;
		rayCastHitMB.layerConstraints = LayerMask.GetMask("Water");

		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitPoint.AddListener(v => hit = v);

		yield return new WaitForEndOfFrame();

		rayCastHitMB.TryHit();

		Assert.AreEqual(new Vector3(0, 0.5f, 0), hit);
	}
}
