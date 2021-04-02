using System;
using UnityEngine;

[RequireComponent(typeof(EffectRunnerMB))]
[RequireComponent(typeof(ResistanceMB))]
public class CharacterSheetMB : MonoBehaviour, ISections
{
	private Resistance resistance;
	private EffectRunnerMB effectRunner;

	public Health health;

	public Action UseSection<TSection>(RefAction<TSection> action, Action fallback)
	{
		return action switch {
			RefAction<Health> use => () => use(ref this.health),
			RefAction<Resistance> use => () => use(ref this.resistance),
			RefAction<EffectRunnerMB> use => () => use(ref this.effectRunner),
			_ => fallback,
		};
	}

	private void Awake()
	{
		this.effectRunner = this.GetComponent<EffectRunnerMB>();
	}

	private void Start()
	{
		this.resistance = this.GetComponent<ResistanceMB>().Resistance;
	}
}
