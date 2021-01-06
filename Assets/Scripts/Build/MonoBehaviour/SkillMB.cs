using System.Collections;
using UnityEngine;

public class SkillMB : MonoBehaviour
{
	private IEnumerator currentBehaviour;

	public CharacterMB agent;
	public Skill skill;
	public BaseSkillBehaviourSO behaviour;

	public void Begin(GameObject target)
	{
		this.currentBehaviour = this.behaviour.Apply(this.agent, this, target);
		this.StartCoroutine(this.currentBehaviour);
	}

	public void End()
	{
		this.StopCoroutine(this.currentBehaviour);
	}
}
