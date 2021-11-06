using NUnit.Framework;
using UnityEngine;

public class BasePhyiscalHitTests : TestCollection
{
	private class MockRayProvider : IRay
	{
		public Ray ray = new Ray(Vector3.up, Vector3.down);
		public Ray Ray => this.ray;
	}

	private class MockMB : MonoBehaviour { }

	private class MockHit : BasePhysicsHit<MockRayProvider> { }

	private MockMB DefaultMockMB
		=> new GameObject("default").AddComponent<MockMB>();

	[Test]
	public void HitNothing() {
		var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };

		rayCastHit.Try(this.DefaultMockMB).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[Test]
	public void HitTarget() {
		var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>();
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.Try(this.DefaultMockMB).Match(
			some: hit => Assert.AreSame(target, hit),
			none: () => Assert.Fail("hit nothing")
		);
	}

	[Test]
	public void HitNothingWhenComponentMissing() {
		var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
		var target = new GameObject("target");
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.Try(this.DefaultMockMB).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[Test]
	public void HitNothingWhenLayersMismatch() {
		var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>(); ;
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.layerConstraints += 1 << 19;
		target.gameObject.layer = 20;

		rayCastHit.Try(this.DefaultMockMB).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[Test]
	public void HitTargetWhenLayersMatch() {
		var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
		var target = new GameObject("target").AddComponent<MockMB>(); ;
		target.gameObject.AddComponent<SphereCollider>();

		rayCastHit.layerConstraints += 1 << 20;
		target.gameObject.layer = 20;

		rayCastHit.Try(this.DefaultMockMB).Match(
			some: hit => Assert.AreSame(target, hit),
			none: () => Assert.Fail("hit nothing")
		);
	}
}
