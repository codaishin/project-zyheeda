using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB<TSheet> :
	MonoBehaviour,
	IHasBegin,
	IHasSheet<TSheet>
{
	private float cooldown;

	[Range(0.1f, 10f)]
	public float applyPerSecond;
	[Range(1, 10)]
	public int maxTargetCount;
	public BaseTargetingSO<TSheet> targeting;

	public TSheet Sheet { get; set; }

	protected abstract void ApplyEffects(TSheet source, TSheet target);
	protected abstract IEnumerator<WaitForFixedUpdate> ApplyCast(TSheet target);

	private IEnumerable<WaitForFixedUpdate> Cast(TSheet target) {
		IEnumerator<WaitForFixedUpdate> routine = this.ApplyCast(target);
		while (routine.MoveNext()) {
			yield return routine.Current;
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerable<WaitForFixedUpdate> AfterCast() {
		while (this.cooldown > 0) {
			yield return new WaitForFixedUpdate();
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerable<WaitForEndOfFrame> SelectTargets(List<TSheet> targets) {
		return this.targeting.Select(this.Sheet, targets, this.maxTargetCount);
	}

	private void StartCooldown(List<TSheet> targets) {
		if (targets.Count > 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
		}
	}

	private IEnumerator RunOn(TSheet target) {
		IEnumerable<WaitForFixedUpdate> cast = this.Cast(target);
		IEnumerable<WaitForFixedUpdate> afterCast = this.AfterCast();
		foreach (WaitForFixedUpdate yield in cast) {
			yield return yield;
		}
		this.ApplyEffects(this.Sheet, target);
		foreach (WaitForFixedUpdate yield in afterCast) {
			yield return yield;
		}
	}

	private void RunOn(List<TSheet> targets) {
		foreach (TSheet target in targets) {
			IEnumerator run = this.RunOn(target);
			this.StartCoroutine(run);
		}
	}

	private IEnumerator Run() {
		List<TSheet> targets = new List<TSheet>();
		foreach (WaitForEndOfFrame yield in this.SelectTargets(targets)) {
			yield return yield;
		}
		this.StartCooldown(targets);
		this.RunOn(targets);
	}

	public void Begin() {
		if (this.cooldown <= 0) {
			IEnumerator run = this.Run();
			this.StartCoroutine(run);
		}
	}
}
