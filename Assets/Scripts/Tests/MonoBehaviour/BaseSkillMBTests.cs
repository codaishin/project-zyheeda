using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockCast : ICast, ISetGameObject
	{
		public delegate IEnumerator<WaitForFixedUpdate> ApplyFunc(GameObject t);

		public ApplyFunc apply = MockCast.BaseApply;

		public GameObject gameObject { get; set; }

		private static IEnumerator<WaitForFixedUpdate> BaseApply(GameObject _) { yield break; }

		public IEnumerator<WaitForFixedUpdate> Apply(GameObject target) =>
			this.apply(target);
	}

	private class MockEffect : IEffect, ISetGameObject
	{
		public delegate void ApplyAction(in GameObject t, in Attributes a);

		public ApplyAction apply = MockEffect.BaseApply;

		public GameObject gameObject { get; set; }

		private static void BaseApply(in GameObject _, in Attributes __) {}

		public void Apply(in GameObject target, in Attributes attributes) =>
			this.apply(target, attributes);
	}

	private class MockSheetMB : MonoBehaviour, IAttributes
	{
		public Attributes attributes;

		public Attributes Attributes => this.attributes;
	}

	private class MockHitableMB : MonoBehaviour, IHitable
	{
		public delegate bool TryHitFunc(in Attributes a);

		public TryHitFunc tryHit = MockHitableMB.BaseTryHit;

		private static bool BaseTryHit(in Attributes _) => true;

		public bool TryHit(in Attributes attributes) =>
			this.tryHit(attributes);
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast> {}

	[Test]
	public void Sheet()
	{
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		Assert.AreSame(sheet, skill.Sheet);
	}

	[Test]
	public void SetCastGameObject()
	{
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		Assert.AreSame(skill.gameObject, skill.cast.gameObject);
	}

	[Test]
	public void SetEffectGameObject()
	{
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		Assert.AreSame(skill.gameObject, skill.effect.gameObject);
	}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new GameObject("target").AddComponent<MockHitableMB>();
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
	public IEnumerator DontBeginWhenTargetNotHitable()
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

		Assert.False(applied);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(GameObject), 0, 0, 0);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		void applyEffect(in GameObject t, in Attributes a) {
			got = (t, a.body, a.mind, a.spirit);
		}

		skill.effect.apply = applyEffect;
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual(
			(target.gameObject, sheet.attributes.body, sheet.attributes.mind, sheet.attributes.spirit),
			got
		);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenTargetNotHitable()
	{
		var got = (default(GameObject), 0, 0, 0);
		var target = new GameObject("target");
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		void applyEffect(in GameObject t, in Attributes a) {
			got = (t, a.body, a.mind, a.spirit);
		}

		skill.effect.apply = applyEffect;
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((default(GameObject), 0, 0, 0), got);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenNotHit()
	{
		var got = (default(GameObject), 0, 0, 0);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		void applyEffect(in GameObject t, in Attributes a) {
			got = (t, a.body, a.mind, a.spirit);
		}
		bool tryHit(in Attributes _) => false;

		skill.effect.apply = applyEffect;
		target.tryHit = tryHit;
		sheet.attributes = new Attributes { body = 1, mind = 2, spirit = 3 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((default(GameObject), 0, 0, 0), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(GameObject, int, int, int)>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got.Add((t, 0, 0, 0));
			yield return new WaitForFixedUpdate();
			got.Add((t, 0, 0, 0));
			yield return new WaitForFixedUpdate();
		}
		void applyEffect(in GameObject t, in Attributes a) {
			got.Add((t, a.body, a.mind, a.spirit));
		}

		skill.cast.apply = applyCast;
		skill.effect.apply = applyEffect;
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
		var target = new GameObject("target").AddComponent<MockHitableMB>();
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
		var target = new GameObject("target").AddComponent<MockHitableMB>();
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
		var target = new GameObject("target").AddComponent<MockHitableMB>();
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
	public IEnumerator ModifiedAttributesInTryHit()
	{
		var got = default(Attributes);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		bool tryHit(in Attributes a) { got = a; return false; }

		skill.modifiers = new Attributes { body = 1, mind = 2, spirit = 3 };
		sheet.attributes = new Attributes { body = 41, mind = 40, spirit = 39 };
		target.tryHit = tryHit;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual(new Attributes{ body = 42, mind = 42, spirit = 42 }, got);
	}

	[UnityTest]
	public IEnumerator ModifiedAttributesInEffect()
	{
		var got = default(Attributes);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var sheet = new GameObject("item").AddComponent<MockSheetMB>();
		var skill = sheet.gameObject.AddComponent<MockSkillMB>();

		void applyEffect(in GameObject _, in Attributes a) { got = a; }

		skill.effect.apply = applyEffect;
		skill.modifiers = new Attributes { body = 1, mind = 2, spirit = 3 };
		sheet.attributes = new Attributes { body = 41, mind = 40, spirit = 39 };

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual(new Attributes{ body = 42, mind = 42, spirit = 42 }, got);
	}
}
