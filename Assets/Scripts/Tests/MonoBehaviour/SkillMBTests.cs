using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkillMBTests : TestCollection
{
	private class MockSkillBehaviourSO : BaseSkillBehaviourSO
	{
		public CharacterMB agent;
		public SkillMB skill;
		public GameObject target;

		public override
		IEnumerator Apply(CharacterMB agent, SkillMB skill, GameObject target)
		{
			this.agent = agent;
			this.target = target;
			this.skill = skill;
			yield break;
		}
	}

	[UnityTest]
	public IEnumerator CallsCorrectAgent()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();

		skill.agent = new GameObject("agent").AddComponent<CharacterMB>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(null);

		Assert.AreSame(skill.agent, behaviour.agent);
	}

	[UnityTest]
	public IEnumerator CallsCorrectTarget()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();
		var target = new GameObject("target");

		skill.agent = new GameObject("agent").AddComponent<CharacterMB>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(target, behaviour.target);
	}

	[UnityTest]
	public IEnumerator CallsCorrectSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();
		var target = new GameObject("target");

		skill.agent = new GameObject("agent").AddComponent<CharacterMB>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(skill, behaviour.skill);
	}
}
