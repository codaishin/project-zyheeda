using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GroundSkillMBTests : TestCollection
{
	private class MockSkillBehaviourSO : BaseSkillBehaviourSO
	{
		public GameObject agent;
		public GameObject target;

		public override void Apply(in GameObject agent, in GameObject target)
		{
			this.agent = agent;
			this.target = target;
		}
	}

	[UnityTest]
	public IEnumerator CallSelector()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.agent = new GameObject("agent");
		skill.behaviour = behaviour;
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Apply(default);

		Assert.AreSame(groundSkill.selector.GameObject, behaviour.target);
	}

	[UnityTest]
	public IEnumerator UpdateSelectorPosition()
	{
		var groundSkill = new GameObject("groundSkill").AddComponent<GroundSkillMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.agent = new GameObject("agent");
		skill.behaviour = behaviour;
		groundSkill.skill = skill;
		groundSkill.selector = new GameObject("selector");

		yield return new WaitForEndOfFrame();

		groundSkill.Apply(Vector3.up);

		Assert.AreEqual(
			Vector3.up,
			groundSkill.selector.GameObject.transform.position
		);
	}
}
