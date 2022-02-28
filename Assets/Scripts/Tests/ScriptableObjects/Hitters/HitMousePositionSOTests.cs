using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class HitMousePositionSOTests : TestCollection
{
	class MockInputConfigSO : BaseInputConfigSO
	{
		public InputAction action = new InputAction(
			binding: "<Mouse>/position",
			type: InputActionType.Value
		);

		public override InputAction this[InputEnum.Action action] => action switch {
			InputEnum.Action when action == InputEnum.Action.MousePosition =>
				this.action,
			_ =>
				throw new ArgumentException(),
		};

		public override InputActionMap this[InputEnum.Map map] =>
			throw new NotImplementedException();
	}

	private Mouse? mouse;

	[SetUp]
	public void SetUp() {
		this.mouse = InputSystem.AddDevice<Mouse>();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.mouse!);
	}

	private class MockRayProvider : IRay
	{
		public Ray ray = new Ray(Vector3.up, Vector3.down);
		public Ray Ray => this.ray;
	}

	private class MockMB : MonoBehaviour { }

	private MockMB DefaultMockMB
		=> new GameObject("default").AddComponent<MockMB>();

	// [Test]
	// public void HitNothing() {
	// 	var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };

	// 	rayCastHit.Try(this.DefaultMockMB).Match(
	// 		some: _ => Assert.Fail("hit something"),
	// 		none: () => Assert.Pass()
	// 	);
	// }

	// [Test]
	// public void HitTarget() {
	// 	var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
	// 	var target = new GameObject("target").AddComponent<MockMB>();
	// 	target.gameObject.AddComponent<SphereCollider>();

	// 	rayCastHit.Try(this.DefaultMockMB).Match(
	// 		some: hit => Assert.AreSame(target, hit),
	// 		none: () => Assert.Fail("hit nothing")
	// 	);
	// }

	// [Test]
	// public void HitNothingWhenComponentMissing() {
	// 	var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
	// 	var target = new GameObject("target");
	// 	target.gameObject.AddComponent<SphereCollider>();

	// 	rayCastHit.Try(this.DefaultMockMB).Match(
	// 		some: _ => Assert.Fail("hit something"),
	// 		none: () => Assert.Pass()
	// 	);
	// }

	// [Test]
	// public void HitNothingWhenLayersMismatch() {
	// 	var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
	// 	var target = new GameObject("target").AddComponent<MockMB>(); ;
	// 	target.gameObject.AddComponent<SphereCollider>();

	// 	rayCastHit.layerConstraints += 1 << 19;
	// 	target.gameObject.layer = 20;

	// 	rayCastHit.Try(this.DefaultMockMB).Match(
	// 		some: _ => Assert.Fail("hit something"),
	// 		none: () => Assert.Pass()
	// 	);
	// }

	// [Test]
	// public void HitTargetWhenLayersMatch() {
	// 	var rayCastHit = new MockHit { rayProvider = new MockRayProvider() };
	// 	var target = new GameObject("target").AddComponent<MockMB>(); ;
	// 	target.gameObject.AddComponent<SphereCollider>();

	// 	rayCastHit.layerConstraints += 1 << 20;
	// 	target.gameObject.layer = 20;

	// 	rayCastHit.Try(this.DefaultMockMB).Match(
	// 		some: hit => Assert.AreSame(target, hit),
	// 		none: () => Assert.Fail("hit nothing")
	// 	);
	// }
}
