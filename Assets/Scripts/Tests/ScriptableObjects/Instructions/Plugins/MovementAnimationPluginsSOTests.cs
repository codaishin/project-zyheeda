using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementAnimationPluginSOTests : TestCollection
{
	class MockMovementAnimationMB : MonoBehaviour, IMovementAnimation
	{
		public Action move = () => { };
		public Action<float> walkOrRunBlend = _ => { };
		public Action stop = () => { };

		public void Begin() => this.move();
		public void WalkOrRunBlend(float value) => this.walkOrRunBlend(value);
		public void End() => this.stop();
	}

	[UnityTest]
	public IEnumerator OnBeginWalk() {
		var called = 0;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.move = () => ++called;

		var action = instructions.GetCallbacks(agent.gameObject).onBegin!;

		yield return new WaitForEndOfFrame();

		action(new PluginData());

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnBeginWalkWeight() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.walkOrRunBlend = value => called = value;

		var action = instructions.GetCallbacks(agent.gameObject).onBegin!;

		yield return new WaitForEndOfFrame();

		action(new PluginData { weight = 0.324f });

		Assert.AreEqual(0.324f, called);
	}

	[UnityTest]
	public IEnumerator OnUpdateWalkWeight() {
		var called = -1f;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.walkOrRunBlend = value => called = value;

		var action = instructions.GetCallbacks(agent.gameObject).onAfterYield!;

		yield return new WaitForEndOfFrame();

		action(new PluginData { weight = 0.111f });

		Assert.AreEqual(0.111f, called);
	}

	[UnityTest]
	public IEnumerator OnEndStop() {
		var called = false;
		var agent = new GameObject().AddComponent<MockMovementAnimationMB>();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.stop = () => called = true;

		var action = instructions.GetCallbacks(agent.gameObject).onEnd!;

		yield return new WaitForEndOfFrame();

		action(new PluginData { weight = 0.324f });

		Assert.True(called);
	}
}
