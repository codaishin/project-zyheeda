using System;
using System.Linq;

public class ChannelListenerMB : BaseListenerMB
{
	public Reference<IChannel>[] listenTo = new Reference<IChannel>[0];
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private Action? applyAll;

	private void InvokeApply() {
		this.applyAll?.Invoke();
	}

	protected override void StartListening() {
		this.listenTo.ForEach(c => c.Value!.AddListener(this.InvokeApply));
	}

	protected override void StopListening() {
		this.listenTo.ForEach(c => c.Value!.RemoveListener(this.InvokeApply));
	}

	protected override void Start() {
		base.Start();
		this.applyAll = this.apply
			.Select(a => a.Value)
			.Aggregate(this.applyAll, (l, c) => l + c!.Apply);
	}
}
