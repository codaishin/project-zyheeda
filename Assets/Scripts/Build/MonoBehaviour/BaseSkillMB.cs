using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IAttributes))]
public abstract class BaseSkillMB<TEffect, TCast> : MonoBehaviour
	where TEffect : IEffect, new()
	where TCast : ICast, new()
{
	private float cooldown;

	public float applyPerSecond;
	public Attributes modifiers;
	public TEffect effect = new TEffect();
	public TCast cast = new TCast();

	public IAttributes Sheet { get; private set; }

	private IEnumerable<WaitForFixedUpdate> Cast(IHitable target)
	{
		IEnumerator<WaitForFixedUpdate> routine = this.cast.Apply(target.gameObject);
		while (routine.MoveNext()) {
			yield return routine.Current;
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private void ApplyEffect(in IHitable target)
	{
		Attributes combined = this.Sheet.Attributes + this.modifiers;
		if (target.TryHit(combined)) {
			this.effect.Apply(target.gameObject, combined);
		}
	}

	private IEnumerable<WaitForFixedUpdate> AfterCast()
	{
		while (this.cooldown > 0) {
			yield return new WaitForFixedUpdate();
			this.cooldown -= Time.fixedDeltaTime;
		}
	}

	private IEnumerator<WaitForFixedUpdate> Apply(IHitable target)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		this.ApplyEffect(target);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public void Begin(GameObject target)
	{
		if (target.TryGetComponent(out IHitable hitable) && this.cooldown <= 0) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Apply(hitable));
		}
	}

	private void Awake()
	{
		this.Sheet = this.RequireComponent<IAttributes>();
	}
}
