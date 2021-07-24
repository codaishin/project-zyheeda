using System;
using UnityEngine;

[RequireComponent(typeof(EffectRunnerMB))]
[RequireComponent(typeof(ResistanceMB))]
[RequireComponent(typeof(EquipmentMB))]
public class CharacterSheetMB : MonoBehaviour, ISections
{
	private EffectRunnerMB effectRunnerMB;
	private ResistanceMB resistanceMB;
	private EquipmentMB equipmentMB;

	public Health health;

	public Action UseSection<TSection>(
		RefAction<TSection> action,
		Action fallback
	) {
		return action switch {
			RefAction<EffectRunnerMB> use => () => use(ref this.effectRunnerMB),
			RefAction<Resistance> use => () => use(ref this.resistanceMB.resistance),
			RefAction<Equipment> use => () => use(ref this.equipmentMB.equipment),
			RefAction<Health> use => () => use(ref this.health),
			_ => fallback,
		};
	}

	private void Awake()
	{
		this.effectRunnerMB = this.GetComponent<EffectRunnerMB>();
		this.resistanceMB = this.GetComponent<ResistanceMB>();
		this.equipmentMB = this.GetComponent<EquipmentMB>();
		this.equipmentMB.sheet = this;
	}
}
