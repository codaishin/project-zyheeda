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

		rayCastHitMB.onHitGameObject.AddListener(o => hit = o);
		rayCastHitMB.TryHit();

		Assert.AreSame(sphere, hit);
	}
}
