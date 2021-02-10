using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private List<IEnumerator> runningRoutines = new List<IEnumerator>();
	private float coolDown;

	public BaseItemBehaviourMB item;
	public Skill data;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => new WaitForFixedUpdate();

	private float CalculateCooldown() => this.data.speedPerSecond == default
		? default
		: 1f / this.data.speedPerSecond;

	private IEnumerator<WaitForFixedUpdate> ApplyTo(GameObject target)
	{
		if (this.item.Apply(this, target, out IEnumerator<WaitForFixedUpdate> routine)) {
			this.coolDown = this.CalculateCooldown();
			while (routine.MoveNext()) {
				yield return routine.Current;
				this.coolDown -= Time.fixedDeltaTime;
			}
			while (this.coolDown > 0) {
				yield return new WaitForFixedUpdate();
				this.coolDown -= Time.fixedDeltaTime;
			}
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
