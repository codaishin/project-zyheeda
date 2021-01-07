using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkillMBTests : TestCollection
{
	private class MockItemMB : BaseItemMB
	{
		public SkillMB skill;
		public GameObject target;
		public int iterations;
		public int applies;

		public override
		IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target)
		{
			this.target = target;
			this.skill = skill;
			++this.applies;
			while (this.iterations < 10) {
				++this.iterations;
				yield return new WaitForFixedUpdate();
			}
		}
	}

	[UnityTest]
	public IEnumerator CallsCorrectTarget()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(target, item.target);
	}

	[UnityTest]
	public IEnumerator CallsCorrectSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(skill, item.skill);
	}

	[UnityTest]
	public IEnumerator BehaviourRunsAsCoroutine()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var ran = new int[2];

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		ran[0] = item.iterations;

		yield return new WaitForFixedUpdate();

		ran[1] = item.iterations;

		CollectionAssert.AreEqual(new int[] { 1, 2 }, ran);
	}

	[UnityTest]
	public IEnumerator StopCoroutine()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var ran = new int[2];

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		ran[0] = item.iterations;
		skill.End();

		yield return new WaitForFixedUpdate();

		ran[1] = item.iterations;

		CollectionAssert.AreEqual(new int[] { 1, 1 }, ran);
	}

	[Test]
	public void ImplementesIPausableWithFixedUpdateYield()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();

		Assert.True(skill is IPausable<WaitForFixedUpdate>);
	}

	[UnityTest]
	public IEnumerator IsPausable()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		skill.Paused = true;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, item.iterations);
	}

	[UnityTest]
	public IEnumerator CanBeUnPaused()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		skill.Paused = true;

		yield return new WaitForFixedUpdate();

		skill.Paused = false;

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.Greater(item.iterations, 1);
	}

	[UnityTest]
	public IEnumerator NoApplyDuringCooldown()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;
		skill.data.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);

		Assert.AreEqual(1, item.applies);
	}

	[UnityTest]
	public IEnumerator CanApplyAfterCooldown()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;
		skill.data.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForSeconds(0.12f);

		skill.Begin(target);

		Assert.AreEqual(2, item.applies);
	}

	[UnityTest]
	public IEnumerator NoApplyDuringCooldownWhenPaused()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;
		skill.data.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Paused = true;

		yield return new WaitForSeconds(0.12f);

		skill.Paused = false;
		skill.Begin(target);

		Assert.AreEqual(1, item.applies);
	}

	[UnityTest]
	public IEnumerator ApplyEndToAllCoroutines()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var item = new GameObject("item").AddComponent<MockItemMB>();

		skill.item = item;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);
		skill.End();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, item.iterations);
	}
}
