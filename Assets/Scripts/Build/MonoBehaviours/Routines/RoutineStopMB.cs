using Routines;
using UnityEngine;

public class RoutineStopMB : MonoBehaviour, IApplicable
{
	public Reference<IStoppable<IFactory>> stopper;
	public int softStopAttempts;

	public Reference<IFactory>[] routineFactories =
		new Reference<IFactory>[0];

	public void Apply() {
		foreach (IFactory factory in this.routineFactories.Values()) {
			this.stopper.Value!.Stop(factory, this.softStopAttempts);
		}
	}

	public void Release() { }
}
