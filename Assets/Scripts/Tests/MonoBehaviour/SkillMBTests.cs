using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkillMBTests : TestCollection
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
	public IEnumerator CallsCorrectAgent()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.agent = new GameObject("agent");
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Apply(null);

		Assert.AreSame(skill.agent.GameObject, behaviour.agent);
	}

	[UnityTest]
	public IEnumerator CallsCorrectTarget()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();
		var target = new GameObject("target");

		skill.agent = new GameObject("agent");
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Apply(target);

		Assert.AreSame(target, behaviour.target);
	}
}
