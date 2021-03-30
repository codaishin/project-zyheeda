using System;

public delegate void ApplyFunc(out Action revert);

public class Effect
{
	public float duration;
	public EffectTag tag;
	public SilenceTag silence;
	public ConditionStacking stacking;

	private Action apply;
	private Action<float> maintain;
	private Action revert;

	public Effect(Action apply = default, Action<float> maintain = default, Action revert = default)
	{
		this.apply = apply;
		this.maintain = maintain;
		this.revert = revert;
	}

	public void Apply()
	{
		if (this.apply != null && this.silence != SilenceTag.ApplyAndRevert) {
			this.apply();
		}
	}

	public void Maintain(float delta)
	{
		if (this.maintain != null && this.silence != SilenceTag.Maintain) {
			this.maintain(delta);
		}
		this.duration -= delta;
	}

	public void Revert()
	{
		if (this.revert != null && this.silence != SilenceTag.ApplyAndRevert) {
			this.revert();
		}
	}
}
