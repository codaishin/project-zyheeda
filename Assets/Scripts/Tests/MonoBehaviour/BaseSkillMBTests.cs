using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockCast : ICast
	{
		public delegate IEnumerator<WaitForFixedUpdate> ApplyFunc(GameObject t);

		public ApplyFunc apply = MockCast.BaseApply;

		private static IEnumerator<WaitForFixedUpdate> BaseApply(GameObject _) { yield break; }

		public IEnumerator<WaitForFixedUpdate> Apply(GameObject target) =>
			this.apply(target);
	}

	private class MockEffect : IEffectCollection<MockSheet>
	{
		public delegate bool GetEffectFunc(GameObject t, out Action<MockSheet> h);

		public GetEffectFunc getEffect = MockEffect.BaseGetEffectFor;

		private static bool BaseGetEffectFor(GameObject _, out Action<MockSheet> h)
		{
			h = _ => {};
			return true;
		}

		public bool GetHandle(GameObject target, out Action<MockSheet> handle) =>
			this.getEffect(target, out handle);
	}

	private class MockSheet : ISheet
	{
		public Attributes attributes;

		public Attributes Attributes => this.attributes;

		public int Hp { get; set; }
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast, MockSheet> {}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			applied = true;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.True(applied);
	}

	[UnityTest]
	public IEnumerator DontBeginWhenGetEffectFalse()
	{
		var got = default(GameObject);
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got = t;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getEffect = (GameObject t, out Action<MockSheet> effect) => {
			effect = a => {};
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.Null(got);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(GameObject), default(MockSheet));
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getEffect = (GameObject t, out Action<MockSheet> handle) => {
			handle = s => got = (t, s);
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((target.gameObject, skill.sheet), got);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenGetEffectFalse()
	{
		var got = (default(GameObject), default(MockSheet));
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getEffect = (GameObject t, out Action<MockSheet> effect) => {
			effect = s => got = (t, s);
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((default(GameObject), default(MockSheet)), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(GameObject, MockSheet)>();
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got.Add((t, default));
			yield return new WaitForFixedUpdate();
			got.Add((t, default));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getEffect = (GameObject t, out Action<MockSheet> effect) => {
			effect = s => got.Add((t, s));
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new (GameObject, MockSheet)[] {
				(target.gameObject, default),
				(target.gameObject, default),
				(target.gameObject, skill.sheet),
			},
			got
		);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);
		skill.Begin(target.gameObject);

		Assert.AreEqual(1, applied);
	}

	[UnityTest]
	public IEnumerator BeginAfterCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForSeconds(0.11f);

		skill.Begin(target.gameObject);

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginWithNoCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);
		skill.Begin(target.gameObject);

		Assert.AreEqual(2, applied);
	}
}
