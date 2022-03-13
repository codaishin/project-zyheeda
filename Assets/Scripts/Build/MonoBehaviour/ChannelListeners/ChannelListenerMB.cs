using UnityEngine.Events;

public class ChannelListenerMB : BaseChannelListenerMB
{
	public Reference<IChannel> listenTo;
	public UnityEvent onRaise = new UnityEvent();

	protected override void StartListening() {
		this.listenTo.Value!.AddListener(onRaise.Invoke);
	}

	protected override void StopListening() {
		this.listenTo.Value!.RemoveListener(onRaise.Invoke);
	}
}
