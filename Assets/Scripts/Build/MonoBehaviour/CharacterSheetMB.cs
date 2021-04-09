using System;
using UnityEngine;

[RequireComponent(typeof(EffectRunnerMB))]
[RequireComponent(typeof(ResistanceMB))]
public class CharacterSheetMB : MonoBehaviour, ISections
{
	private ResistanceMB resistanceMB;
	private EffectRunnerMB effectRunnerMB;

	public Health health;

	public Action UseSection<TSection>(RefAction<TSection> action, Action fallback)
	{
		return action switch {
			RefAction<Health> use => () => use(ref this.health),
			RefAction<Resistance> use => () => use(ref this.resistanceMB.resistance),
			RefAction<EffectRunnerMB> use => () => use(ref this.effectRunnerMB),
			_ => fallback,
		};
	}

	private void Awake()
	{
		this.effectRunnerMB = this.GetComponent<EffectRunnerMB>();
		this.resistanceMB = this.GetComponent<ResistanceMB>();
	}
}
