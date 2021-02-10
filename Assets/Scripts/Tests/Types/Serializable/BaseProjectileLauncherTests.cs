using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseProjectileLauncherTests
{
	private class MockProjectilePathing : IProjectilePathing
	{
		public int iterations;
		public Transform target;

		public
		IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target)
		{
			this.target = target;
			for (int i = 0; i < this.iterations; ++i) {
				yield return new WaitForFixedUpdate();
			}
		}
	}

	private class MockLauncherBehaviour : BaseProjectileLauncher<MockProjectilePathing> { }

	private class MockSkillMB : BaseSkillMB<MockLauncherBehaviour> {}

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
		public BaseSkillMB skill;
		public GameObject target;

		public override void Apply(in BaseSkillMB skill, in GameObject target)
		{
			this.skill = skill;
			this.target = target;
		}
	}

	[Test]
	public void InitProjectilePath()
	{
		var launcher = new MockLauncherBehaviour();
		Assert.NotNull(launcher.projectilePathing);
	}

	[Test]
	public void CallTryHit()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		skill.modifiers.offense = 42;
		launcher.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreEqual(42, target.usedOffense);
	}

	[Test]
	public void CallTryHitValid()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		skill.modifiers.offense = 42;
		var valid = launcher.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(valid);
	}

	[Test]
	public void UseProjectilePathing()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		launcher.projectilePathing.iterations = 5;
		launcher.Apply(skill, target.gameObject, out var routine);
		var count = 0;
		while (routine.MoveNext()) {
			++count;
		}
		Assert.AreEqual(5, count);
	}

	[Test]
	public void UseProjectilePathingToTarget()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		launcher.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext());

		Assert.AreSame(target.transform, launcher.projectilePathing.target);
	}

	[Test]
	public void NoProjectilePathing()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target");

		launcher.projectilePathing.iterations = 5;
		var valid = launcher.Apply(skill, target.gameObject, out var routine);
		Assert.AreEqual(
			(null as IEnumerator<WaitForFixedUpdate>, false),
			(routine, valid)
		);
	}

	[Test]
	public void ApplyEffects()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};

		target.hit = true;
		skill.effects = effects;
		launcher.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		CollectionAssert.AreEqual(
			new (BaseSkillMB, GameObject)[] {
				(skill, target.gameObject),
				(skill, target.gameObject),
			},
			effects.Select(e => (e.skill, e.target))
		);
	}

	[Test]
	public void DontApplyEffects()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};

		skill.effects = effects;
		launcher.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(effects.All(e => !e.skill && !e.target));
	}

	[Test]
	public void ApplyEffectsOnlyAfterProjectileHit()
	{
		var launcher = new MockLauncherBehaviour();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var applied = new List<bool>();

		target.hit = true;
		skill.effects = effects;
		launcher.projectilePathing.iterations = 2;
		launcher.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext()) {
			applied.Add(effects.All(e => e.skill || e.target));
		}
		applied.Add(effects.All(e => e.skill || e.target));

		CollectionAssert.AreEqual(new bool[] { false, false, true }, applied);
	}
}
