using UnityEngine;

public abstract class BaseRunnerMB<TBeginnable> :
	MonoBehaviour,
	IHasBegin,
	IHasValue<TBeginnable>
	where TBeginnable :
		class,
		IHasBegin
{
	public TBeginnable Value { get; set; }

	public void Begin() => this.Value.Begin();
}
