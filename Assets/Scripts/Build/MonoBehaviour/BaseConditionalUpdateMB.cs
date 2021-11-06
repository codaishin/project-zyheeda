using UnityEngine;
using UnityEngine.Events;

public class BaseConditionalUpdateMB<TValue, TConditional> :
	MonoBehaviour
	where TConditional :
		IConditional<TValue>
{
	public TValue? value;
	public TConditional? conditional;
	public UnityEvent? onUpdate;

	private void Start() {
		if (this.onUpdate == null) {
			this.onUpdate = new UnityEvent();
		}
	}

	private void Update() {
		if (this.conditional == null || this.value == null) throw this.NullError();
		if (this.conditional.Check(this.value)) {
			this.onUpdate?.Invoke();
		}
	}
}
