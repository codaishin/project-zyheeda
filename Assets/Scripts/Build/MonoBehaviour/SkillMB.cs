using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private List<IEnumerator> runningRoutines = new List<IEnumerator>();
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
			IEnumerator routine = this.Manage(this.behaviour.Apply(this.agent, this, target));
			this.StartCoroutine(routine);
			this.lastBegun = Time.fixedTime;
			this.runningRoutines.Add(routine);
		}
	}

	public void End()
	{
		this.runningRoutines.ForEach(this.StopCoroutine);
		this.runningRoutines.Clear();
	}
}
