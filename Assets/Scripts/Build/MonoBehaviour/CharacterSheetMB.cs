using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetMB : MonoBehaviour, ISections
{
	public Health health;

	public Action UseSection<TSection>(
		RefAction<TSection> action,
		Action? fallback = default
	) {
		return action switch {
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

	private void Awake() { }
}
