using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB<TEffectCollection, TCast, TSheet> : MonoBehaviour, ISkill<TSheet>
	where TEffectCollection : IEffectCollection<TSheet>, new()
	where TCast : ICast<TSheet>, new()
{
	private float cooldown;

	[Range(0.1f, 10f)]
	public float applyPerSecond;
	[Range(1, 10)]
	public int maxTargetCount;
	public BaseTargetingSO<TSheet> targeting;
	public TEffectCollection effectCollection;
	public TCast cast;

	public TSheet Sheet { get; set; }

	private IEnumerable<WaitForFixedUpdate> Cast(TSheet target)
	{
		IEnumerator<WaitForFixedUpdate> routine = this.cast.Apply(target);
		while (routine.MoveNext()) {
			yield return routine.Current;
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerable<WaitForFixedUpdate> AfterCast()
	{
		while (this.cooldown > 0) {
			yield return new WaitForFixedUpdate();
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerable<WaitForEndOfFrame> SelectTargets(List<TSheet> targets)
	{
		IEnumerator<WaitForEndOfFrame> routine = this.targeting.Select(
			this.Sheet,
			targets,
			this.maxTargetCount
		);
		while (routine.MoveNext()) {
			yield return routine.Current;
		}
	}

	private void StartCooldown(List<TSheet> targets)
	{
		if (targets.Count > 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
		}
	}

	private IEnumerator RunOn(TSheet target)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		this.effectCollection.Apply(this.Sheet, target);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	private IEnumerator Run()
	{
		List<TSheet> targets = new List<TSheet>();
		foreach (WaitForEndOfFrame yield in this.SelectTargets(targets)) {
			yield return yield;
		}
		this.StartCooldown(targets);
		foreach (TSheet target in targets) {
			this.StartCoroutine(this.RunOn(target));
		}
	}

	public void Begin()
	{
		if (this.cooldown <= 0) {
			this.StartCoroutine(this.Run());
		}
	}

	private void Awake()
	{
		if (this.cast == null) {
			this.cast = new TCast();
		}
		if (this.effectCollection == null) {
			this.effectCollection = new TEffectCollection();
		}
	}
}
