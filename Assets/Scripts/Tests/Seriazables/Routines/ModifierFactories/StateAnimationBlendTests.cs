using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class StateAnimationBlendTests : TestCollection
	{
		class MockAnimationMB : MonoBehaviour, IAnimationStatesBlend
		{
			public Action<Animation.BlendState, float> blend = (_, __) => { };

			public void Blend(Animation.BlendState state, float weight) =>
				this.blend(state, weight);
		}

		[UnityTest]
		public IEnumerator WalkWeight() {
			var called = ((Animation.BlendState)(-1), -1f);
			var agent = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData { weight = 0.324f };
			var plugin = new StateAnimationBlend();

			agent.blend = (state, value) => called = (state, value);
			plugin.blendState = (Animation.BlendState)(42);

			var blend = plugin.GetModifierFnFor(agent.gameObject)(data)!;

			yield return new WaitForEndOfFrame();

			blend();

			Assert.AreEqual((plugin.blendState, 0.324f), called);
		}

		[UnityTest]
		public IEnumerator WalkWeightOnChild() {
			var called = ((Animation.BlendState)(-1), -1f);
			var agent = new GameObject();
			var child = new GameObject().AddComponent<MockAnimationMB>();
			var data = new WeightData { weight = 0.111f };
			var plugin = new StateAnimationBlend();

			child.blend = (state, value) => called = (state, value);
			child.transform.parent = agent.transform;
			plugin.blendState = (Animation.BlendState)(33);

			var blend = plugin.GetModifierFnFor(agent.gameObject)(data)!;

			yield return new WaitForEndOfFrame();

			blend();

			Assert.AreEqual((plugin.blendState, 0.111f), called);
		}
	}
}
