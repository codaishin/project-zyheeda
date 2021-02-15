using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ProjectileLauncherSOTests : TestCollection
{
	private class Mock
	{
		public int iterations;
		public Transform Target {get; private set; }

		public
		IEnumerator<WaitForFixedUpdate> ProjectileRoutine(Transform agent, Transform target)
		{
			this.Target = target;
			for (int i = 0; i < this.iterations; ++i) {
				yield return new WaitForFixedUpdate();
			}
		}
	}

	private class MockHitableMB : BaseHitableMB
	{
		public int usedOffense;
		public bool hit;

		public override bool TryHit(in int offense)
		{
			this.usedOffense = offense;
			return this.hit;
		}
	}

	private class MockEffectSO : BaseEffectSO
	{
		public SkillMB skill;
		public GameObject target;

		public override void Apply(in SkillMB skill, in GameObject target)
		{
			this.skill = skill;
			this.target = target;
		}
	}

	[Test]
	public void BaseProjectilePath()
	{
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		Assert.Fail();
	}

	[Test]
	public void CallTryHit()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		skill.modifiers.offense = 42;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreEqual(42, target.usedOffense);
	}

	[Test]
	public void CallTryHitValid()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		var valid = behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(valid);
	}

	[Test]
	public void UseProjectilePathing()
	{
		var count = 0;
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldFive(Transform _, Transform __) {
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		};

		behaviour.projectileRoutine = yieldFive;
		behaviour.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext()) {
			++count;
		}
		Assert.AreEqual(5, count);
	}

	[Test]
	public void UseProjectilePathingToTarget()
	{
		var got = null as Transform;
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform t) {
			got = t;
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreSame(target.transform, got);
	}

	[Test]
	public void NoProjectilePathing()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		var valid = behaviour.Apply(skill, target, out var routine);
		Assert.AreEqual(
			(null as IEnumerator<WaitForFixedUpdate>, false),
			(routine, valid)
		);
	}

	[Test]
	public void ApplyEffects()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __) {
			yield break;
		};

		target.hit = true;
		skill.effects = effects;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		CollectionAssert.AreEqual(
			new (SkillMB, GameObject)[] {
				(skill, target.gameObject),
				(skill, target.gameObject),
			},
			effects.Select(e => (e.skill, e.target))
		);
	}

	[Test]
	public void DontApplyEffects()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __) {
			yield break;
		};

		skill.effects = effects;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(effects.All(e => !e.skill && !e.target));
	}

	[Test]
	public void ApplyEffectsOnlyAfterProjectileHit()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var applied = new List<bool>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		IEnumerator<WaitForFixedUpdate> yieldTwo(Transform _, Transform __) {
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		};

		target.hit = true;
		skill.effects = effects;
		behaviour.projectileRoutine = yieldTwo;
		behaviour.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext()) {
			applied.Add(effects.All(e => e.skill || e.target));
		}
		applied.Add(effects.All(e => e.skill || e.target));

		CollectionAssert.AreEqual(new bool[] { false, false, true }, applied);
	}
}
