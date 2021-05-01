using UnityEngine;

public class InputLayerSwitchMB : MonoBehaviour
{
	public InputLayerSO inputLayer;
	public InputLayer setTo;
	public EventSO[] listenTo;

	private void Start()
	{
		foreach (EventSO e in this.listenTo) {
			e.Listeners += this.Set;
		}
	}

	private void Set()
	{
		this.inputLayer.CurrentInputLayer = this.setTo;
	}
}
