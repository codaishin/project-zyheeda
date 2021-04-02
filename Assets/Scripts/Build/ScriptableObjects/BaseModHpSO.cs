using System;

public class BaseModHpSO<TResistance> : BaseEffectFactorySO
	where TResistance : ISimpleDict<EffectTag, float>
{
	public bool invert = true;

	private Action<float> GetModHp<TSheet>(TSheet target)
		where TSheet : ISections
	{
		float compliance = default;
		float modifier = default;

		Action updateCompliance = target.UseSection<TResistance>(
			(ref TResistance resistance) => compliance = 1f - resistance[this.tag],
			() => {}
		);
		Action modifyHp = target.UseSection<Health>(
			(ref Health health) => health.hp = this.invert ? health.hp - modifier : health.hp + modifier,
			() => {}
		);

		return intensity => {
			modifier = intensity;
			updateCompliance();
			modifier *= compliance;
			modifyHp();
		};
	}

	public override Effect Create<TSheet>(TSheet source, TSheet target, float intensity)
	{
		Action<float> modHp = this.GetModHp(target);
		return new Effect(
			apply: () => modHp(intensity),
			maintain: (float intervalDelta) => modHp(intensity * intervalDelta)
		);
	}
}
