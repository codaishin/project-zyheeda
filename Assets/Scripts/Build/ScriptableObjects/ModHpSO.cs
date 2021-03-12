using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EffectBehaviour/ModHp")]
public class ModHpSO : BaseEffectBehaviourSO
{
	private
	void ModHp<TSheet>(TSheet source, TSheet target, int intensity) where TSheet : ISections
	{
		target.UseSection<Health>((ref Health health) => health.hp -= intensity);
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
