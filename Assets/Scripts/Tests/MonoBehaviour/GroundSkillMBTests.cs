using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GroundSkillMBTests : TestCollection
{
	private class MockItemBehaviour : BaseItemBehaviour
	{
		public GameObject target;

		public override
		bool Apply(BaseSkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			IEnumerator<WaitForFixedUpdate> empty() {
				yield break;
			}
			this.target = target;
			routine = empty();
			return default;
		}
	}

	private class MockSkillMB : BaseSkillMB<MockItemBehaviour> {}

	[UnityTest]
	public IEnumerator CallSelector()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();

		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(default);

		Assert.AreSame(groundSkill.selector, skill.behaviour.target);
	}

	[UnityTest]
	public IEnumerator UpdateSelectorPosition()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();

		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(Vector3.up);

		Assert.AreEqual(Vector3.up, groundSkill.selector.transform.position);
	}
}
