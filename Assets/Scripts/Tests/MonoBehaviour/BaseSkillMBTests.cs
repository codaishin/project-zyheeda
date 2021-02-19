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

	private class MockEffect : IEffect
	{
		public delegate bool GetEffectFunc(GameObject t, out EffectFunc e);

		public GetEffectFunc getEffect = MockEffect.BaseGetEffectFor;

		private static bool BaseGetEffectFor(GameObject _, out EffectFunc e)
		{
			e = (in Attributes _) => {};
			return true;
		}

		public bool GetEffect(GameObject target, out EffectFunc effect) =>
			this.getEffect(target, out effect);
	}

	private class MockSheetMB : MonoBehaviour, ISheet
	{
		public Attributes attributes;

		public Attributes Attributes => this.attributes;

		public int Hp { get; set; }
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast> {}

	[Test]
	public void Sheet()
	{
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		Assert.AreSame(sheet, skill.Sheet);
	}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

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
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got = t;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.effect.getEffect = (GameObject t, out EffectFunc effect) => {
			effect = (in Attributes a) => {};
			return false;
		};
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.Null(got);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(GameObject), 0, 0, 0);
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		skill.effect.getEffect = (GameObject t, out EffectFunc effect) => {
			effect = (in Attributes a) => got = (t, a.body, a.mind, a.spirit);
			return true;
		};
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual(
			(target.gameObject, sheet.attributes.body, sheet.attributes.mind, sheet.attributes.spirit),
			got
		);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenGetEffectFalse()
	{
		var got = (default(GameObject), 0, 0, 0);
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		skill.effect.getEffect = (GameObject t, out EffectFunc effect) => {
			effect = (in Attributes a) => got = (t, a.body, a.mind, a.spirit);
			return false;
		};
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((default(GameObject), 0, 0, 0), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(GameObject, int, int, int)>();
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got.Add((t, 0, 0, 0));
			yield return new WaitForFixedUpdate();
			got.Add((t, 0, 0, 0));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effect.getEffect = (GameObject t, out EffectFunc effect) => {
			effect = (in Attributes a) => got.Add((t, a.body, a.mind, a.spirit));
			return true;
		};
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new (GameObject, int, int, int)[] {
				(target.gameObject, 0, 0, 0),
				(target.gameObject, 0, 0, 0),
				(target.gameObject, 1, 2, 3),
			},
			got
		);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

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
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

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
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

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

	[UnityTest]
	public IEnumerator ModifiedAttributesInEffect()
	{
		var got = default(Attributes);
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		skill.effect.getEffect = (GameObject _, out EffectFunc effect) => {
			effect = (in Attributes a) => got = a;
			return true;
		};
		skill.modifiers = new Attributes { body = 1, mind = 2, spirit = 3 };
		sheet.attributes = new Attributes { body = 41, mind = 40, spirit = 39 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual(new Attributes{ body = 42, mind = 42, spirit = 42 }, got);
	}
}
