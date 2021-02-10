using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GroundSkillMBTests : TestCollection
{
	private class MockItemMB : BaseItemBehaviourMB
	{
		public SkillMB skill;
		public GameObject target;
		public bool valid;

		public override
		bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			IEnumerator<WaitForFixedUpdate> empty() {
				yield break;
			}
			this.target = target;
			this.skill = skill;
			routine = empty();
			return this.valid;
		}
	}

	[UnityTest]
	public IEnumerator CallSelector()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var item = new GameObject("agent").AddComponent<MockItemMB>();

		skill.item = item;
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(default);

		Assert.AreSame(groundSkill.selector, item.target);
	}

	[UnityTest]
	public IEnumerator UpdateSelectorPosition()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();

		skill.item = new GameObject("agent").AddComponent<MockItemMB>();
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(Vector3.up);

		Assert.AreEqual(Vector3.up, groundSkill.selector.transform.position);
	}
}
