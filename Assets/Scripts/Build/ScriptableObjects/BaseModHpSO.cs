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
		void modifyIntensity(ref TResistance resistance) => intensity = (int)(intensity * (1f - resistance[this.tag]));
		void modifyHp(ref Health health) => health.hp = this.invert switch {
			true => health.hp - intensity,
			false => health.hp + intensity,
		};

		target.UseSection<TResistance>(modifyIntensity);
		target.UseSection<Health>(modifyHp);
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
