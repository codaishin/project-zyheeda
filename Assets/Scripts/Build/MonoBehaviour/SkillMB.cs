using System.Collections;
using UnityEngine;

public class SkillMB : MonoBehaviour
{
	public CharacterMB agent;
	public Skill skill;
	public BaseSkillBehaviourSO behaviour;

	public void Begin(GameObject target)
	{
		this.StartCoroutine(this.behaviour.Apply(this.agent, this, target));
	}
}
