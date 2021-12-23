using UnityEngine.Events;

public abstract class ValueChannelListenerMB<T> : BaseChannelListenerMB
{
	public ValueChannelSO<T>? listenTo;
	public UnityEvent<T> onRaise = new UnityEvent<T>();

	protected override void StartListening() {
		this.listenTo!.Listeners += this.onRaise.Invoke;
	}

	protected override void StopListening() {
		this.listenTo!.Listeners -= this.onRaise.Invoke;
	}
}
