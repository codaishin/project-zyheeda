using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StateAnimationTests : TestCollection
{
	class MockAnimatorMB : MonoBehaviour, IAnimationStates
	{
		public Action<Animation.State> set = (_) => { };
		public void Set(Animation.State state) => this.set(state);
	}

	[UnityTest]
	public IEnumerator StateOnBegin() {
		var agent = new GameObject().AddComponent<MockAnimatorMB>();
		var plugin = new StateAnimation();
		var called = Animation.State.Idle;

		plugin.beginState = Animation.State.ShootRifle;
		agent.set = s => called = s;

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent.gameObject)!;

		callbacks(new PluginData()).onBegin!();

		Assert.AreEqual(Animation.State.ShootRifle, called);
	}

	[UnityTest]
	public IEnumerator StateOnBeginOnChild() {
		var agent = new GameObject();
		var child = new GameObject().AddComponent<MockAnimatorMB>();
		var plugin = new StateAnimation();
		var called = Animation.State.Idle;

		plugin.beginState = Animation.State.WalkOrRun;
		child.set = s => called = s;
		child.transform.parent = agent.transform;

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent)!;

		callbacks(new PluginData()).onBegin!();

		Assert.AreEqual(Animation.State.WalkOrRun, called);
	}

	[UnityTest]
	public IEnumerator StateOnEnd() {
		var agent = new GameObject().AddComponent<MockAnimatorMB>();
		var plugin = new StateAnimation();
		var called = Animation.State.WalkOrRun;

		plugin.endState = Animation.State.Idle;
		agent.set = s => called = s;

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent.gameObject)!;

		callbacks(new PluginData()).onEnd!();

		Assert.AreEqual(Animation.State.Idle, called);
	}
}
