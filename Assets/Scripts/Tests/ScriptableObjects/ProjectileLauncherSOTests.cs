using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ProjectileLauncherSOTests : TestCollection
{
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

	private SkillMB MockSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var item = new GameObject("item");
		skill.transform.SetParent(item.transform);
		return skill;
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
	public void BaseProjectileRoutine()
	{
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var routine = (ProjectileRoutineFunc)ProjectileRoutine.Create;
		Assert.AreEqual(routine, behaviour.projectileRoutine);
	}

	[Test]
	public void CallTryHitValid()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
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
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldFive(Transform _, Transform __, float ___) {
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
	public void Parameters()
	{
		var got = (null as Transform, null as Transform, 0f);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform f, Transform t, float d) {
			got = (f, t, d);
			yield break;
		};

		behaviour.deltaPerSecond = 10f;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreEqual((skill.Item.transform, target.transform, 10f), got);
	}

	[Test]
	public void NoProjectilePathing()
	{
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		var valid = behaviour.Apply(skill, target, out var routine);
		Assert.AreEqual(
			(null as IEnumerator<WaitForFixedUpdate>, false),
			(routine, valid)
		);
	}
}
