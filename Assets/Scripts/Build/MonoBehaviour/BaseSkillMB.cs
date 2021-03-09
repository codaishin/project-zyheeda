using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB<TEffectCollection, TCast, TSheet> : MonoBehaviour
	where TEffectCollection : IEffectCollection<TSheet>, new()
	where TCast : ICast, new()
{
	private float cooldown;

	public TSheet sheet;
	public float applyPerSecond;
	public TEffectCollection effectCollection;
	public TCast cast;

	private IEnumerable<WaitForFixedUpdate> Cast(GameObject target)
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

	private IEnumerator<WaitForFixedUpdate> Apply(GameObject target, Action applyEffects)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		applyEffects();
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public void Begin(GameObject target)
	{
		if (this.effectCollection.GetApplyEffects(this.sheet, target, out Action applyEffects) && this.cooldown <= 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Apply(target, applyEffects));
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
