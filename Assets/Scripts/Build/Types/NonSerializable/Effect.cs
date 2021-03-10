using System;

public class Effect
{
	private enum State { Idle = default, Active }

	private State state;
	public float duration;
	public EffectTag tag;
	public ConditionStacking stacking;

	public event Action OnApply;
	public event Action<float> OnMaintain;
	public event Action OnRevert;

	public void Apply()
	{
		if (this.state == State.Idle) {
			this.OnApply?.Invoke();
			this.state = State.Active;
		}
	}

	public void Maintain(float delta)
	{
		if (this.state == State.Active) {
			this.duration -= delta;
			this.OnMaintain?.Invoke(delta);
		}
	}

	public void Revert()
	{
		if (this.state == State.Active) {
			this.OnRevert?.Invoke();
		}
	}
}
