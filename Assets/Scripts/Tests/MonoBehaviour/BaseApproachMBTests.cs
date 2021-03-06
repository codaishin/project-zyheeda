using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseApproachMBTests : TestCollection
{
	private class MockApproach : IApproach<Vector3>
	{
		public Func<Transform, Vector3, float, IEnumerator<WaitForFixedUpdate>> apply =
			MockApproach.DefaultApproach;

		public IEnumerator<WaitForFixedUpdate> Apply(Transform transform, Vector3 target, float speed) =>
			this.apply(transform, target, speed);

		private static IEnumerator<WaitForFixedUpdate> DefaultApproach(Transform t, Vector3 v, float f) { yield break; }
	}

	private class MockApproachMB : BaseApproachMB<MockApproach> {}

	[UnityTest]
	public IEnumerator CallApproach()
	{
		var called = default((Transform, Vector3, float));
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 4;

		IEnumerator<WaitForFixedUpdate> approach(Transform transform, Vector3 target, float speed) {
			called = (transform, target, speed);
			yield break;
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		mover.Apply(Vector3.right);

		Assert.AreEqual((agent.transform, Vector3.right, 4f), called);
	}

	[UnityTest]
	public IEnumerator KeepCallingApproach()
	{
		var called = 0;
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(Transform transform, Vector3 target, float speed) {
			++called;
			yield return new WaitForFixedUpdate();
			++called;
			yield return new WaitForFixedUpdate();
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();

		mover.Apply(Vector3.right);

		yield return new WaitForSeconds(0.5f);

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator MoveOverrideOldMove()
	{
		var called = new List<Vector3>();
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(Transform transform, Vector3 target, float speed) {
			called.Add(target);
			yield return new WaitForFixedUpdate();
			called.Add(target);
			yield return new WaitForFixedUpdate();
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();

		mover.Apply(Vector3.right);
		mover.Apply(Vector3.left);

		yield return new WaitForSeconds(0.5f);

		CollectionAssert.AreEqual(
			new Vector3[] { Vector3.right, Vector3.left, Vector3.left },
			called
		);
	}
}
