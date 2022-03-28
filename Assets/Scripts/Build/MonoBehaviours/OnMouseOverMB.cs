using UnityEngine;
using UnityEngine.Events;

public class OnMouseOverMB : MonoBehaviour
{
	public UnityEvent? onMouseEnter;
	public UnityEvent? onMouseExit;

	private void Start() {
		if (this.onMouseEnter == null) {
			this.onMouseEnter = new UnityEvent();
		}
		if (this.onMouseExit == null) {
			this.onMouseExit = new UnityEvent();
		}
	}

	private void OnMouseEnter() => this.onMouseEnter!.Invoke();

	private void OnMouseExit() => this.onMouseExit!.Invoke();
}
