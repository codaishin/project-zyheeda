using NUnit.Framework;
using UnityEngine;

public class BaseHitTests : TestCollection
{
	private class MockRayProvider : IRay
	{
		public Ray ray = new Ray(Vector3.up, Vector3.down);
		public Ray Ray => this.ray;
	}

	private class MockMB : MonoBehaviour {}

	private class MockHit : BaseHit<MockRayProvider> {}

	[Test]
	public void HitNothing()
	{
		var rayCastHit = new MockHit{ rayProvider = new MockRayProvider() };
		Assert.AreEqual((false, default(MockMB)), (rayCastHit.TryHit(out MockMB t), t));
	}

	[Test]
	public void HitTarget()
	{
		var rayCastHit = new MockHit{ rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>();
		target.gameObject.AddComponent<SphereCollider>();

		Assert.AreEqual((true, target), (rayCastHit.TryHit(out MockMB t), t));
	}

	[Test]
	public void HitNothingWhenComponentMissing()
	{
		var rayCastHit = new MockHit{ rayProvider = new MockRayProvider() };
		var target = new GameObject("target");
		target.gameObject.AddComponent<SphereCollider>();

		Assert.AreEqual((false, default(MockMB)), (rayCastHit.TryHit(out MockMB t), t));
	}

	[Test]
	public void HitNothingWhenLayersMismatch()
	{
		var rayCastHit = new MockHit{ rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>();;
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.layerConstraints += 1 << 19;
		target.gameObject.layer = 20;

		Assert.AreEqual((false, default(MockMB)), (rayCastHit.TryHit(out MockMB t), t));
	}

	[Test]
	public void HitTargetWhenLayersMatch()
	{
		var rayCastHit = new MockHit{ rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>();;
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.layerConstraints += 1 << 20;
		target.gameObject.layer = 20;

		Assert.AreEqual((true, target), (rayCastHit.TryHit(out MockMB t), t));
	}
}
