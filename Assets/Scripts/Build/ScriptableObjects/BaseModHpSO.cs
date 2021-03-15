using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EffectBehaviour/ModHp")]
public class BaseModHpSO<TResistance> : BaseEffectBehaviourSO
	where TResistance : IResistance
{
	public bool invert = true;

	private
	void ModHp<TSheet>(TSheet source, TSheet target, int intensity) where TSheet : ISections
	{
		void mod(ref Health health) => health.hp = this.invert
			? health.hp - intensity
			: health.hp + intensity;

		target.UseSection<Health>(mod);
	}

	public override
	void Apply<TSheet>(TSheet source, TSheet target, int intensity)
	{
		this.ModHp(source, target, intensity);
	}

	public override
	void Maintain<TSheet>(TSheet source, TSheet target, int intensity, float intervalDelta)
	{
		this.ModHp(source, target, (int)(intensity * intervalDelta));
	}

	public override
	void Revert<TSheet>(TSheet source, TSheet target, int intensity) { }
}
