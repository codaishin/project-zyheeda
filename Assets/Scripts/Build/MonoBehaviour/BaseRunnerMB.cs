using UnityEngine;

public abstract class BaseRunnerMB<TBeginnable> :
	MonoBehaviour,
	IHasBegin,
	IHasValue<TBeginnable>
	where TBeginnable :
		class,
		IHasBegin
{
	private TBeginnable? value;

	public TBeginnable Value {
		get => this.value ?? throw this.NullError();
		set => this.value = value;
	}

	public void Begin() => this.Value.Begin();
}
