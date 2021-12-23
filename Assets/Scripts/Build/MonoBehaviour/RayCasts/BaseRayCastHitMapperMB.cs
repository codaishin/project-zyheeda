using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRayCastHitMaperMB<TRayCastHit, TOut> :
	MonoBehaviour
	where TRayCastHit :
		MonoBehaviour,
		IRayCastHit
{
	private TRayCastHit? rayCastHit;

	public UnityEvent<TOut> onValueMapped = new UnityEvent<TOut>();

	protected void RunEvent(TOut value) => this.onValueMapped.Invoke(value);
	protected void DoNothing() { }

	public void Apply() =>
		this
			.Map(this.rayCastHit!.TryHit())
			.Match(some: this.RunEvent, none: this.DoNothing);

	public abstract Maybe<TOut> Map(Maybe<RaycastHit> hit);

	private void Start() {
		this.rayCastHit = this.RequireComponent<TRayCastHit>();
	}
}
