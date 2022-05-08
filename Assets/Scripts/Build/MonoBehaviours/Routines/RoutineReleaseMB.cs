using Routines;
using UnityEngine;

public class RoutineReleaseMB : MonoBehaviour, IApplicable
{
	public Reference<IApplicable<IFactory>> runner;

	public Reference<IFactory>[] routineFactories =
		new Reference<IFactory>[0];

	public void Apply() {
		foreach (IFactory factory in this.routineFactories.Values()) {
			this.runner.Value!.Release(factory);
		}
	}

	public void Release() { }
}
