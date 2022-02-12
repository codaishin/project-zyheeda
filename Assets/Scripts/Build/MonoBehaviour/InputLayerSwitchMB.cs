using System;
using UnityEngine;

public class InputLayerSwitchMB : MonoBehaviour
{
	public InputLayerSO? inputLayer;
	public InputLayer setTo;
	public BaseChannelSO[]? listenTo;

	private void Start() {
		if (this.listenTo == null) throw this.NullError();
		if (this.inputLayer == null) throw this.NullError();
		foreach (BaseChannelSO e in this.listenTo) {
			e.AddListener(this.Set(this.inputLayer));
		}
	}

	private Action Set(InputLayerSO inputLayer) {
		return () => inputLayer.CurrentInputLayer = this.setTo;
	}
}
