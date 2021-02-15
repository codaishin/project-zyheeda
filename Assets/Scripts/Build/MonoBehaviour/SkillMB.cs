using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private List<IEnumerator> runningRoutines = new List<IEnumerator>();
	private float coolDown;

	public Skill modifiers;
	public BaseEffectSO[] effects = new BaseEffectSO[0];
	public BaseItemBehaviourSO behaviour;

	public bool Paused { get; set; }
	public WaitForFixedUpdate Pause => new WaitForFixedUpdate();
	public GameObject Item => this.transform.parent.gameObject;

	private float FullCooldown => this.modifiers.speedPerSecond == default
		? default
		: 1f / this.modifiers.speedPerSecond;

	private IEnumerable<WaitForFixedUpdate> Cast(IEnumerator<WaitForFixedUpdate> routine)
	{
		this.coolDown = this.FullCooldown;
		while (routine.MoveNext()) {
			yield return routine.Current;
			this.coolDown -= Time.fixedDeltaTime;
		}
	}

	private void ApplyEffects(GameObject target)
	{
		if (target.TryGetComponent(out BaseHitableMB hitable) && hitable.TryHit(this.modifiers.offense)) {
			this.effects.ForEach(e => e.Apply(this, target));
		}
	}

	private IEnumerable<WaitForFixedUpdate> AfterCast()
	{
		while (this.coolDown > 0) {
			yield return new WaitForFixedUpdate();
			this.coolDown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerator<WaitForFixedUpdate> ApplyTo(GameObject target)
	{
		if (this.behaviour.Apply(this, target, out IEnumerator<WaitForFixedUpdate> routine)) {
			foreach (WaitForFixedUpdate yield in this.Cast(routine)) {
				yield return yield;
			}
			this.ApplyEffects(target);
			foreach (WaitForFixedUpdate yield in this.AfterCast()) {
				yield return yield;
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
