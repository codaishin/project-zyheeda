using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	public Skill modifiers;
	public BaseEffectSO[] effects;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => new WaitForFixedUpdate();

	public abstract void Begin(GameObject target);
	public abstract void End();
}

public abstract class BaseSkillMB<T> : BaseSkillMB
	where T: IItemBehaviour, new()
{
	private List<IEnumerator> runningRoutines = new List<IEnumerator>();
	private float coolDown;

	public T behaviour = new T();

	private float CalculateCooldown() => this.modifiers.speedPerSecond == default
		? default
		: 1f / this.modifiers.speedPerSecond;

	private IEnumerator<WaitForFixedUpdate> ApplyTo(GameObject target)
	{
		if (this.behaviour.Apply(this, target, out IEnumerator<WaitForFixedUpdate> routine)) {
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

	public override void Begin(GameObject target)
	{
		if (this.coolDown <= 0) {
			IEnumerator routine = this.Manage(this.ApplyTo(target));
			this.StartCoroutine(routine);
			this.runningRoutines.Add(routine);
		}
	}

	public override void End()
	{
		this.runningRoutines.ForEach(this.StopCoroutine);
		this.runningRoutines.Clear();
	}
}
