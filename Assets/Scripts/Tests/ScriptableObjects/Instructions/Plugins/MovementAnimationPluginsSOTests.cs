using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementAnimationPluginSOTests : TestCollection
{
	class MockMovementAnimationMB : MonoBehaviour, IMovementAnimation
	{
		public Action<float> move = _ => { };
		public Action stop = () => { };

		public void Move(float walkOrRunWeight) => this.move(walkOrRunWeight);
		public void Stop() => this.stop();
	}

	[UnityTest]
	public IEnumerator OnBeginWalk() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.move = value => called = value;

		var action = instructions.GetOnBegin(agent.gameObject, new PluginData());

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator OnBeginWalkWeight() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.move = value => called = value;

		var action = instructions.GetOnBegin(
			agent.gameObject,
			new PluginData { weight = 0.324f }
		);

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(0.324f, called);
	}

	[UnityTest]
	public IEnumerator OnUpdateWalk() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.move = value => called = value;

		var action = instructions.GetOnUpdate(agent.gameObject, new PluginData());

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator OnUpdateWalkWeight() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.move = value => called = value;

		var action = instructions.GetOnUpdate(
			agent.gameObject,
			new PluginData { weight = 0.111f }
		);

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(0.111f, called);
	}

	[UnityTest]
	public IEnumerator OnEndStop() {
		var called = false;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.stop = () => called = true;

		var action = instructions.GetOnEnd(
			agent.gameObject,
			new PluginData { weight = 0.324f }
		);

		yield return new WaitForEndOfFrame();

		action();

		Assert.True(called);
	}
}
