using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB<TEffect, TCast> : MonoBehaviour
	where TEffect : IEffect, new()
	where TCast : ICast, new()
{
	private IItem item;
	private float cooldown;

	public float applyPerSecond;
	public Attributes modifiers;
	public TEffect effect = new TEffect();
	public TCast cast = new TCast();

	private IEnumerable<WaitForFixedUpdate> Cast(GameObject target)
	{
		IEnumerator<WaitForFixedUpdate> routine = this.cast.Apply(target);
		while (routine.MoveNext()) {
			yield return routine.Current;
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private void ApplyEffect(in GameObject target, in IHitable hitable)
	{
		Attributes combined = this.item.Attributes + this.modifiers;
		if (hitable.TryHit(combined)) {
			this.effect.Apply(target, combined);
		}
	}

	private IEnumerable<WaitForFixedUpdate> AfterCast()
	{
		while (this.cooldown > 0) {
			yield return new WaitForFixedUpdate();
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerator<WaitForFixedUpdate> Apply(GameObject target, IHitable hitable)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		this.ApplyEffect(target, hitable);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public void Begin(GameObject target)
	{
		if (this.cast.Valid(target, out IHitable hitable) && this.cooldown <= 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Apply(target, hitable));
		}
	}

	private void Start()
	{
		this.item = this.transform.parent.RequireComponent<IItem>();
	}
}
