using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class StateAnimationTests : TestCollection
	{
		class MockAnimatorMB : MonoBehaviour, IAnimationStates
		{
			public Action<Animation.State> set = (_) => { };
			public void Set(Animation.State state) => this.set(state);
		}

		[UnityTest]
		public IEnumerator Statebegin() {
			var agent = new GameObject().AddComponent<MockAnimatorMB>();
			var plugin = new StateAnimation();
			var called = Animation.State.Idle;

			plugin.state = Animation.State.ShootRifle;
			agent.set = s => called = s;

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetModifierFnFor(agent.gameObject);
			var setState = modifierFn(new Data())!;

			setState();

			Assert.AreEqual(Animation.State.ShootRifle, called);
		}

		[UnityTest]
		public IEnumerator StatebeginOnChild() {
			var agent = new GameObject();
			var child = new GameObject().AddComponent<MockAnimatorMB>();
			var plugin = new StateAnimation();
			var called = Animation.State.Idle;

			plugin.state = Animation.State.WalkOrRun;
			child.set = s => called = s;
			child.transform.parent = agent.transform;

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetModifierFnFor(agent.gameObject);
			var setState = modifierFn(new Data())!;

			setState();

			Assert.AreEqual(Animation.State.WalkOrRun, called);
		}
	}
}
