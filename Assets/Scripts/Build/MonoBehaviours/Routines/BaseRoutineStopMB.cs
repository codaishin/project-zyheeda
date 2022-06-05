using UnityEngine;

public abstract class BaseRoutineStopMB<TKey> :
	MonoBehaviour,
	IApplicable
{
	public Reference<IStoppable<TKey>> stopper;
	public int softStopAttempts;

	public TKey[] keys = new TKey[0];

	public void Apply() {
		foreach (var key in this.keys) {
			this.stopper.Value!.Stop(key, this.softStopAttempts);
		}
	}
}
