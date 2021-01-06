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

		public int iterations;

		public override
		IEnumerator Apply(CharacterMB agent, SkillMB skill, GameObject target)
		{
			this.agent = agent;
			this.target = target;
			this.skill = skill;
			while (this.iterations < 100) {
				++this.iterations;
				yield return new WaitForFixedUpdate();
			}
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

	[UnityTest]
	public IEnumerator BehaviourRunsAsCoroutine()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockSkillBehaviourSO>();
		var target = new GameObject("target");
		var ran = new int[2];

		skill.agent = new GameObject("agent").AddComponent<CharacterMB>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		ran[0] = behaviour.iterations;

		yield return new WaitForFixedUpdate();

		ran[1] = behaviour.iterations;

		CollectionAssert.AreEqual(new int[] { 1, 2 }, ran);
	}
}
