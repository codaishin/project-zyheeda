using System;

public delegate void ApplyFunc(out Action revert);

public class Effect
{
	public float duration;
	public EffectTag tag;
	public SilenceTag silence;
	public ConditionStacking stacking;

	private event ApplyFunc apply;
	private event Action<float> maintain;

	public Effect(ApplyFunc apply = default, Action<float> maintain = default)
	{
		this.apply = apply;
		this.maintain = maintain;
	}

	public bool Apply(out Action revert)
	{
		revert = default;
		if (this.apply != null && this.silence != SilenceTag.ApplyAndRevert) {
			this.apply(out revert);
		}
		return revert != default;
	}

	public void Maintain(float delta)
	{
		if (this.maintain != null && this.silence != SilenceTag.Maintain) {
			this.maintain(delta);
		}
		this.duration -= delta;
	}
}
