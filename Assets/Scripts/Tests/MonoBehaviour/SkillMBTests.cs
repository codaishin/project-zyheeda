using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkillMBTests : TestCollection
{
	private delegate IEnumerable<WaitForEndOfFrame> SelectFn(
		MockSheet sheet,
		List<MockSheet> targets,
		int maxCount
	);

	class MockSheet { }

	class MockTargetingSO : BaseTargetingSO<MockSheet>
	{
		public MockSheet[] targets = new MockSheet[0];

		protected override IEnumerable<WaitForEndOfFrame> DoSelect(
			MockSheet source,
			List<MockSheet> targets,
			int maxCount
		) {
			foreach (MockSheet target in this.targets) {
				targets.Add(target);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	class MockCast : ICast<MockSheet>
	{
		public Func<MockSheet, IEnumerator<WaitForFixedUpdate>> apply
			= _ => Enumerable.Empty<WaitForFixedUpdate>().GetEnumerator();

		public IEnumerator<WaitForFixedUpdate> Apply(
			MockSheet target
		) => this.apply(target);
	}

	class MockEffects : IEffectCollection<MockSheet>
	{
		public Action<MockSheet, MockSheet> apply
			= (_, __) => { };

		public void Apply(
			MockSheet source,
			MockSheet target
		) => this.apply(source, target);
	}

	class MockSkillMB : SkillMB<MockSheet, MockEffects, MockCast>
	{
		private void Start() {
			this.Sheet = new MockSheet();
		}
	}


	[UnityTest]
	public IEnumerator ApplyCast() {
		var called = default(MockSheet);
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new MockSheet();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		targeting.targets = new MockSheet[] { target };
		skill.targeting = targeting;
		skill.cast.apply = (source) => {
			called = source;
			return Enumerable.Empty<WaitForFixedUpdate>().GetEnumerator();
		};

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(target, called);
	}

	[UnityTest]
	public IEnumerator UseCastIterator() {
		var stepps = 0;
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new MockSheet();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		targeting.targets = new MockSheet[] { target };
		skill.targeting = targeting;
		skill.cast.apply = (_) => {
			IEnumerator<WaitForFixedUpdate> iterator() {
				for (int i = 0; i < 2; ++i) {
					++stepps;
					yield return new WaitForFixedUpdate();
				}
			}
			return iterator();
		};

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, stepps);
	}


	[UnityTest]
	public IEnumerator ApplyEffects() {
		var called = new List<(MockSheet, MockSheet)>();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var targetA = new MockSheet();
		var targetB = new MockSheet();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		targeting.targets = new MockSheet[] { targetA, targetB };
		skill.targeting = targeting;
		skill.effectCollection.apply
			= (source, target) => called.Add((source, target));

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			new (MockSheet, MockSheet)[] {
				(skill.Sheet, targetA),
				(skill.Sheet, targetB),
			},
			called
		);
	}
}
