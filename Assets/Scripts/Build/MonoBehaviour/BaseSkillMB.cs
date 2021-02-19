using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB : MonoBehaviour
{
	public abstract void Begin(GameObject target);
}

[RequireComponent(typeof(ISheet))]
public abstract class BaseSkillMB<TEffect, TCast> : BaseSkillMB
	where TEffect : IEffect, new()
	where TCast : ICast, new()
{
	private ISheet sheet;
	private float cooldown;

	public float applyPerSecond;
	public Attributes modifiers;
	public TEffect effect;
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

	private IEnumerator<WaitForFixedUpdate> Apply(GameObject target, EffectFunc effect)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		effect(this.sheet.Attributes + this.modifiers);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public override void Begin(GameObject target)
	{
		if (this.effect.GetEffect(target, out EffectFunc effect) && this.cooldown <= 0) {
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
		if (this.effect == null) {
			this.effect = new TEffect();
		}
	}
}
