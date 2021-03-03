using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB : MonoBehaviour
{
	public abstract void Begin(GameObject target);
}

[RequireComponent(typeof(ISheet))]
public abstract class BaseSkillMB<TEffectCollection, TCast> : BaseSkillMB
	where TEffectCollection : IEffectCollection, new()
	where TCast : ICast, new()
{
	private ISheet sheet;
	private float cooldown;

	public float applyPerSecond;
	public Attributes modifiers;
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

	private IEnumerator<WaitForFixedUpdate> Apply(GameObject target, Action<Attributes> handle)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		handle(this.sheet.Attributes + this.modifiers);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public override void Begin(GameObject target)
	{
		if (this.effectCollection.GetHandle(target, out Action<Attributes> effect) && this.cooldown <= 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Apply(target, effect));
		}
	}

	private void Awake()
	{
		this.sheet = this.RequireComponent<ISheet>();
		if (this.cast == null) {
			this.cast = new TCast();
		}
		if (this.effectCollection == null) {
			this.effectCollection = new TEffectCollection();
		}
	}
}
