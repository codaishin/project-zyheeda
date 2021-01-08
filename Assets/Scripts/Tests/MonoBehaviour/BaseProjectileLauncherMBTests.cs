using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
}
