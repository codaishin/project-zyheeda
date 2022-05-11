using UnityEngine;

public class RoutineApplyMB : MonoBehaviour, IApplicable
{
	public Reference<IApplicable<Routines.IFactory>> runner;
	public Reference<Routines.IFactory> routineFactory;

	public void Apply() {
		this.runner.Value!.Apply(this.routineFactory.Value!);
	}
}
