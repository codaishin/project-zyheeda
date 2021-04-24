using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillMB<TEffectCollection, TCast, TSheet> : MonoBehaviour, ISkill<TSheet>
	where TEffectCollection : IEffectCollection<TSheet>, new()
	where TCast : ICast<TSheet>, new()
{
	private float cooldown;

	public BaseHitSO hitter;
	public float applyPerSecond;
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

	private IEnumerator<WaitForFixedUpdate> Run(TSheet target)
	{
		foreach (WaitForFixedUpdate yield in this.Cast(target)) {
			yield return yield;
		}
		this.effectCollection.Apply(this.Sheet, target);
		foreach (WaitForFixedUpdate yield in this.AfterCast()) {
			yield return yield;
		}
	}

	public void Begin()
	{
		if (this.cooldown <= 0 && this.hitter.Hit.TryHit(this.Sheet, out TSheet target)) {
			this.cooldown = this.applyPerSecond > 0 ? 1f / this.applyPerSecond : 0;
			this.StartCoroutine(this.Run(target));
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
