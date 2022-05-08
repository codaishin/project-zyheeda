using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class MovementAnimationTests : TestCollection
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
		public IEnumerator beginWalk() {
			var called = (Animation.State)(-1);
			var agent = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData();
			var plugin = new MovementAnimation();

			agent.set = s => called = s;

			var action = plugin
				.GetModifierFnFor(agent.gameObject)(data)
				.begin!;

			yield return new WaitForEndOfFrame();

			action();

			Assert.AreEqual(Animation.State.WalkOrRun, called);
		}

		[UnityTest]
		public IEnumerator beginWalkWithAnimationOnChild() {
			var called = (Animation.State)(-1);
			var agent = new GameObject();
			var child = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData();
			var plugin = new MovementAnimation();

			child.transform.parent = agent.transform;
			child.set = s => called = s;

			var action = plugin
				.GetModifierFnFor(agent.gameObject)(data)
				.begin!;

			yield return new WaitForEndOfFrame();

			action();

			Assert.AreEqual(Animation.State.WalkOrRun, called);
		}

		[UnityTest]
		public IEnumerator beginWalkWeight() {
			var called = ((Animation.BlendState)(-1), -1f);
			var agent = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData { weight = 0.324f };
			var plugin = new MovementAnimation();

			agent.blend = (state, value) => called = (state, value);

			var action = plugin
				.GetModifierFnFor(agent.gameObject)(data)
				.begin!;

			yield return new WaitForEndOfFrame();

			action();

			Assert.AreEqual((Animation.BlendState.WalkOrRun, 0.324f), called);
		}

		[UnityTest]
		public IEnumerator updateWalkWeight() {
			var called = ((Animation.BlendState)(-1), -1f);
			var agent = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData { weight = 0.111f };
			var plugin = new MovementAnimation();

			agent.blend = (state, value) => called = (state, value);

			var action = plugin
				.GetModifierFnFor(agent.gameObject)(data)
				.update!;

			yield return new WaitForEndOfFrame();

			action();

			Assert.AreEqual((Animation.BlendState.WalkOrRun, 0.111f), called);
		}

		[UnityTest]
		public IEnumerator endStop() {
			var called = (Animation.State)(-1);
			var agent = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData { weight = 0.324f };
			var plugin = new MovementAnimation();

			agent.set = s => called = s;

			var action = plugin
				.GetModifierFnFor(agent.gameObject)(data)
				.end!;

			yield return new WaitForEndOfFrame();

			action();

			Assert.AreEqual(Animation.State.Idle, called);
		}
	}
}
