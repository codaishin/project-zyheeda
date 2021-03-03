using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB : MonoBehaviour
{
	public abstract void Begin(GameObject target);
}

public abstract class BaseSkillMB<TEffectCollection, TCast, TSheet> : BaseSkillMB
	where TEffectCollection : IEffectCollection<TSheet>, new()
	where TCast : ICast, new()
	where TSheet : ISheet
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

	private IEnumerator<WaitForFixedUpdate> Apply(GameObject target, Action<TSheet> handle)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		handle(this.sheet);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public override void Begin(GameObject target)
	{
		if (this.effectCollection.GetHandle(target, out Action<TSheet> effect) && this.cooldown <= 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Apply(target, effect));
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
