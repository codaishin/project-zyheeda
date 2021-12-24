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
		public Func<Transform, Vector3, float, IEnumerator<WaitForFixedUpdate>> apply = MockApproach.DefaultApproach;

		public IEnumerator<WaitForFixedUpdate> Apply(
			Transform transform,
			Vector3 target,
			float speed
		) => this.apply(transform, target, speed);

		private static IEnumerator<WaitForFixedUpdate> DefaultApproach(
			Transform t,
			Vector3 v,
			float f
		) {
			yield break;
		}
	}

	private class MockApproachMB : BaseApproachMB<MockApproach, Vector3> { }

	[UnityTest]
	public IEnumerator CallApproach() {
		var called = default((Transform, Vector3, float));
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;
		mover.deltaPerSecond = 4;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			called = (transform, target, speed);
			yield break;
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		mover.Begin(Vector3.right);

		Assert.AreEqual((agent.transform, Vector3.right, 4f), called);
	}

	[UnityTest]
	public IEnumerator KeepCallingApproach() {
		var called = 0;
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			++called;
			yield return new WaitForFixedUpdate();
			++called;
			yield return new WaitForFixedUpdate();
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();

		mover.Begin(Vector3.right);

		yield return new WaitForSeconds(0.5f);

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator MoveOverrideOldMove() {
		var called = new List<Vector3>();
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			called.Add(target);
			yield return new WaitForFixedUpdate();
			called.Add(target);
			yield return new WaitForFixedUpdate();
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();

		mover.Begin(Vector3.right);
		mover.Begin(Vector3.left);

		yield return new WaitForSeconds(0.5f);

		CollectionAssert.AreEqual(
			new Vector3[] { Vector3.right, Vector3.left, Vector3.left },
			called
		);
	}

	[UnityTest]
	public IEnumerator OnEnd() {
		var called = 0;
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		}
		mover.appraoch.apply = approach;

		yield return new WaitForEndOfFrame();

		mover.onEnd.AddListener(() => ++called);
		mover.Begin(new Vector3(100, 0, 0));

		Assert.AreEqual(0, called);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator RunCoroutinesOnExternalRunner() {
		var agent = new GameObject("agent");
		var runner = new GameObject("runner").AddComponent<CoroutineRunnerMB>();
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.externalRunner = runner;
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			while (transform.position != target) {
				yield return new WaitForFixedUpdate();
				transform.position = Vector3.MoveTowards(
					transform.position,
					target,
					1f
				);
			}
		}
		mover.appraoch.apply = approach;

		yield return new WaitForFixedUpdate();

		mover.Begin(new Vector3(100, 0, 0));

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(new Vector3(1, 0, 0), agent.transform.position);

		runner.StopAllCoroutines();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(new Vector3(1, 0, 0), agent.transform.position);
	}

	[UnityTest]
	public IEnumerator RunCoroutinesOnThisIfNoRunner() {
		var agent = new GameObject("agent");
		var mover = new GameObject("mover").AddComponent<MockApproachMB>();
		mover.agent = agent;

		IEnumerator<WaitForFixedUpdate> approach(
			Transform transform,
			Vector3 target,
			float speed
		) {
			transform.position = target;
			yield break;
		}
		mover.appraoch.apply = approach;

		yield return new WaitForFixedUpdate();

		mover.Begin(new Vector3(100, 0, 0));

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(new Vector3(100, 0, 0), agent.transform.position);
	}
}
