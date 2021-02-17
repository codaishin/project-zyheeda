using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GroundSkillMBTests : TestCollection
{
	private class MockSkillMB : BaseSkillMB
	{
		public GameObject target { get; private set; }

		public override void Begin(GameObject target) => this.target = target;
	}

	[UnityTest]
	public IEnumerator CallSelector()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<MockSkillMB>();

		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Begin(default);

		Assert.AreSame(groundSkill.selector, skill.target);
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
