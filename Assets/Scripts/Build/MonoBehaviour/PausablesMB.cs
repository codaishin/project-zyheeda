using UnityEngine;

public class PausablesMB : MonoBehaviour
{
	private IPausable[]? pausables;

	private void Start() {
		this.pausables = this.GetComponentsInChildren<IPausable>();
	}

	public void PauseChildren(bool value) =>
		this.pausables!.ForEach(p => p.Paused = value);
}
