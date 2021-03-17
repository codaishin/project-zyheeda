using System;

public class BaseModHpSO<TSheet, TResistance> : BaseEffectFactorySO<TSheet>
	where TSheet : ISections
	where TResistance : IResistance
{
	public bool invert = true;

	private Action<float> GetModHp(TSheet target)
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

	public override Effect Create(TSheet source, TSheet target, float intensity)
	{
		Action<float> modHp = this.GetModHp(target);
		return new Effect(
			apply: (out Action reverse) => {
				modHp(intensity);
				reverse = default;
			},
			maintain: (float intervalDelta) => {
				modHp(intensity * intervalDelta);
			}
		);
	}
}
