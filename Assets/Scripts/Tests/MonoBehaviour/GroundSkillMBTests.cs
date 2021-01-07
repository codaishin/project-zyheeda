using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GroundSkillMBTests : TestCollection
{
	private class MockSkillBehaviourSO : BaseFixedUpdateSkillBehaviourSO
	{
		public ItemMB item;
		public SkillMB skill;
		public GameObject target;

		public override
		IEnumerator<WaitForFixedUpdate> Apply(ItemMB item, SkillMB skill, GameObject target)
		{
			this.item = item;
			this.target = target;
			this.skill = skill;
			yield break;
		}
	}

	[UnityTest]
	public IEnumerator CallSelector()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.item = new GameObject("agent").AddComponent<ItemMB>();
		skill.behaviour = behaviour;
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(default);

		Assert.AreSame(groundSkill.selector, behaviour.target);
	}

	[UnityTest]
	public IEnumerator UpdateSelectorPosition()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.item = new GameObject("agent").AddComponent<ItemMB>();
		skill.behaviour = behaviour;
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(Vector3.up);

		Assert.AreEqual(Vector3.up, groundSkill.selector.transform.position);
	}
}
