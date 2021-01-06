using System.Collections;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private IEnumerator currentBehaviour;

	public CharacterMB agent;
	public Skill skill;
	public BaseSkillBehaviourSO behaviour;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => default;

	public void Begin(GameObject target)
	{
		this.currentBehaviour = this.Manage(this.behaviour.Apply(this.agent, this, target));
		this.StartCoroutine(this.currentBehaviour);
	}

	public void End()
	{
		this.StopCoroutine(this.currentBehaviour);
	}
}
