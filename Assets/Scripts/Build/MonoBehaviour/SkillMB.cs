using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private List<IEnumerator> runningRoutines = new List<IEnumerator>();
	private float coolDown;

	public CharacterMB agent;
	public Skill skill;
	public BaseFixedUpdateSkillBehaviourSO behaviour;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => default;

	private float CalculateCooldown() => this.skill.appliesPerSecond == default
		? default
		: 1f / this.skill.appliesPerSecond;

	private IEnumerator<WaitForFixedUpdate> ApplyTo(GameObject target)
	{
		IEnumerator<WaitForFixedUpdate> iterator =
			this.behaviour.Apply(this.agent, this, target);
		this.coolDown = this.CalculateCooldown();
		while (iterator.MoveNext()) {
			yield return iterator.Current;
			this.coolDown -= Time.fixedDeltaTime;
		}
	}

	public void Begin(GameObject target)
	{
		if (this.coolDown <= 0) {
			IEnumerator routine = this.Manage(this.ApplyTo(target));
			this.StartCoroutine(routine);
			this.runningRoutines.Add(routine);
		}
	}

	public void End()
	{
		this.runningRoutines.ForEach(this.StopCoroutine);
		this.runningRoutines.Clear();
	}
}
