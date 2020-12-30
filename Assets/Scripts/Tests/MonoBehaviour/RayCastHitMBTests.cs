using System.Collections;
using System.Collections.Generic;
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
	public IEnumerator OnHitGameObjectInitAfterStart()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHitGameObject);
	}

	[UnityTest]
	public IEnumerator OnHitGameObjectDefaultNull()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();

		Assert.Null(rayCastHitMB.onHitGameObject);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitGameObject()
	{
		var hit = null as GameObject;
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.rayProviderMB = rayProviderMB;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitGameObject.AddListener(o => hit = o);
		rayCastHitMB.TryHit();

		Assert.AreSame(sphere, hit);
	}

	[UnityTest]
	public IEnumerator OnHitVector3InitAfterStart()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(rayCastHitMB.onHitVector3);
	}

	[UnityTest]
	public IEnumerator OnHitVector3DefaultNull()
	{
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();

		Assert.Null(rayCastHitMB.onHitVector3);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnHitVector3Object()
	{
		var hit = Vector3.zero;
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		var rayCastHitMB = new GameObject("hitter").AddComponent<RayCastHitMB>();
		var rayProviderMB = new GameObject("ray").AddComponent<MockRayMB>();

		sphere.transform.position = Vector3.up;
		rayProviderMB.ray = new Ray(Vector3.up * 2, Vector3.down);
		rayCastHitMB.rayProviderMB = rayProviderMB;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		rayCastHitMB.onHitVector3.AddListener(v => hit = v);
		rayCastHitMB.TryHit();

		Assert.AreEqual(new Vector3(0, 1.5f, 0), hit);
	}
}
