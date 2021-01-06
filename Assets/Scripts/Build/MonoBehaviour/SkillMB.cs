using System.Collections;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private IEnumerator currentBehaviour;
	private float lastBegun;

	public CharacterMB agent;
	public Skill skill;
	public BaseFixedUpdateSkillBehaviourSO behaviour;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => default;

	public void Begin(GameObject target)
	{
		float cooldown = this.skill.appliesPerSecond == 0
			? 0
			: 1f / this.skill.appliesPerSecond;
		if (Time.fixedTime - this.lastBegun >= cooldown) {
			this.lastBegun = Time.fixedTime;
			this.currentBehaviour = this.Manage(this.behaviour.Apply(this.agent, this, target));
			this.StartCoroutine(this.currentBehaviour);
		}
	}

	public void End()
	{
		this.StopCoroutine(this.currentBehaviour);
	}
}
