using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementAnimationPluginSOTests : TestCollection
{
	class MockAnimationMB : MonoBehaviour, IAnimationStates, IAnimationStatesBlend
	{
		public Action<Animation.State> set = _ => { };
		public Action<Animation.BlendState, float> blend = (_, __) => { };

		public void Set(Animation.State state) =>
			this.set(state);
		public void Blend(Animation.BlendState state, float weight) =>
			this.blend(state, weight);
	}

	[UnityTest]
	public IEnumerator OnBeginWalk() {
		var called = (Animation.State)(-1);
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var pluginData = new CorePluginData();
		var instructions = ScriptableObject
			.CreateInstance<MovementAnimationPluginSO>();

		agent.set = s => called = s;

		var action = instructions
			.GetCallbacks(agent.gameObject)(pluginData)
			.onBegin!;

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(Animation.State.WalkOrRun, called);
	}

	[UnityTest]
	public IEnumerator OnBeginWalkWithAnimationOnChild() {
		var called = (Animation.State)(-1);
		var agent = new GameObject();
		var child = new GameObject().AddComponent<MockAnimationMB>();
		var pluginData = new CorePluginData();
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		child.transform.parent = agent.transform;
		child.set = s => called = s;

		var action = instructions
			.GetCallbacks(agent.gameObject)(pluginData)
			.onBegin!;

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(Animation.State.WalkOrRun, called);
	}

	[UnityTest]
	public IEnumerator OnBeginWalkWeight() {
		var called = ((Animation.BlendState)(-1), -1f);
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var pluginData = new CorePluginData { weight = 0.324f };
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.blend = (state, value) => called = (state, value);

		var action = instructions
			.GetCallbacks(agent.gameObject)(pluginData)
			.onBegin!;

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual((Animation.BlendState.WalkOrRun, 0.324f), called);
	}

	[UnityTest]
	public IEnumerator OnUpdateWalkWeight() {
		var called = ((Animation.BlendState)(-1), -1f);
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var pluginData = new CorePluginData { weight = 0.111f };
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.blend = (state, value) => called = (state, value);

		var action = instructions
			.GetCallbacks(agent.gameObject)(pluginData)
			.onAfterYield!;

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual((Animation.BlendState.WalkOrRun, 0.111f), called);
	}

	[UnityTest]
	public IEnumerator OnEndStop() {
		var called = (Animation.State)(-1);
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var pluginData = new CorePluginData { weight = 0.324f };
		var instructions = ScriptableObject.CreateInstance<MovementAnimationPluginSO>();

		agent.set = s => called = s;

		var action = instructions
			.GetCallbacks(agent.gameObject)(pluginData)
			.onEnd!;

		yield return new WaitForEndOfFrame();

		action();

		Assert.AreEqual(Animation.State.Idle, called);
	}
}
