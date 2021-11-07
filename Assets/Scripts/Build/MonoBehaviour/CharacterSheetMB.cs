using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EffectRunnerMB))]
[RequireComponent(typeof(ResistanceMB))]
[RequireComponent(typeof(EquipmentMB))]
public class CharacterSheetMB : MonoBehaviour, ISections
{
	private IEffectRunner? effectRunner;
	private ResistanceMB? resistanceMB;
	private EquipmentMB? equipmentMB;

	public Health health;

	public Action UseSection<TSection>(
		RefAction<TSection> action,
		Action? fallback = default
	) {
		return action switch {
			RefAction<IEffectRunner> use
				=> () => use(ref this.effectRunner!),
			RefAction<Resistance> use when this.resistanceMB!.resistance != null
				=> () => use(ref this.resistanceMB!.resistance),
			RefAction<Equipment> use
				=> () => use(ref this.equipmentMB!.equipment),
			RefAction<Health> use
				=> () => use(ref this.health),
			_ => this.UseOrThrow<TSection>(fallback),
		};
	}

	private Action UseOrThrow<TSection>(Action? fallback) {
		return fallback ?? throw new KeyNotFoundException(
			$"no Section of type {typeof(TSection)} on {this}"
		);
	}

	private void Awake() {
		this.effectRunner = this.GetComponent<EffectRunnerMB>();
		this.resistanceMB = this.GetComponent<ResistanceMB>();
		this.equipmentMB = this.GetComponent<EquipmentMB>();
		this.equipmentMB.sheet = this;
	}
}
