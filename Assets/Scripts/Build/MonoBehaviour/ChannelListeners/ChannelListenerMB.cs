using UnityEngine.Events;

public class ChannelListenerMB : BaseChannelListenerMB
{
	public ChannelSO? listenTo;
	public UnityEvent onRaise = new UnityEvent();

	protected override void StartListening() {
		this.listenTo!.AddListener(onRaise.Invoke);
	}

	protected override void StopListening() {
		this.listenTo!.RemoveListener(onRaise.Invoke);
	}
}
