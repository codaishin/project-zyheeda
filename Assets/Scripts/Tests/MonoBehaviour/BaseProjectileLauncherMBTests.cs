using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseProjectileLauncherMBTests
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

	private class MockLauncherMB :
		BaseProjectileLauncherMB<MockProjectilePathing> { }

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

	private class MockEffectMB : BaseEffectMB
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
	public void InitProjectilePath()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		Assert.NotNull(launcher.projectilePathing);
	}

	[Test]
	public void CallTryHit()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		skill.data.offense = 42;
		launcher.Apply(skill, target.gameObject).MoveNext();

		Assert.AreEqual(42, target.usedOffense);
	}

	[Test]
	public void UseProjectilePathing()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		launcher.projectilePathing.iterations = 5;
		var iterator = launcher.Apply(skill, target.gameObject);
		var count = 0;
		while (iterator.MoveNext()) {
			++count;
		}
		Assert.AreEqual(5, count);
	}

	[Test]
	public void UseProjectilePathingToTarget()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();

		var iterator = launcher.Apply(skill, target.gameObject);
		while (iterator.MoveNext());

		Assert.AreSame(target.transform, launcher.projectilePathing.target);
	}

	[Test]
	public void NoProjectilePathing()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");

		launcher.projectilePathing.iterations = 5;
		var iterator = launcher.Apply(skill, target.gameObject);
		var count = 0;
		while (iterator.MoveNext()) {
			++count;
		}
		Assert.AreEqual(0, count);
	}

	[Test]
	public void ApplyEffects()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectMB[] {
			launcher.gameObject.AddComponent<MockEffectMB>(),
			launcher.gameObject.AddComponent<MockEffectMB>(),
		};

		target.hit = true;
		launcher.Apply(skill, target.gameObject).MoveNext();

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
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectMB[] {
			launcher.gameObject.AddComponent<MockEffectMB>(),
			launcher.gameObject.AddComponent<MockEffectMB>(),
		};

		launcher.Apply(skill, target.gameObject).MoveNext();

		Assert.True(effects.All(e => !e.skill && !e.target));
	}

	[Test]
	public void ApplyEffectsOnlyAfterProjectileHit()
	{
		var launcher = new GameObject("launcher").AddComponent<MockLauncherMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectMB[] {
			launcher.gameObject.AddComponent<MockEffectMB>(),
			launcher.gameObject.AddComponent<MockEffectMB>(),
		};
		var applied = new List<bool>();

		target.hit = true;
		launcher.projectilePathing.iterations = 2;
		var iterator = launcher.Apply(skill, target.gameObject);
		while (iterator.MoveNext()) {
			applied.Add(effects.All(e => e.skill || e.target));
		}
		applied.Add(effects.All(e => e.skill || e.target));

		CollectionAssert.AreEqual(new bool[] { false, false, true }, applied);
	}
}
