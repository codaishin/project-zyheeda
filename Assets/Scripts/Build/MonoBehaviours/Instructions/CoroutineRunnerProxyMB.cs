using UnityEngine;

public class CoroutineRunnerProxyMB : MonoBehaviour, IApplicable
{
	public CoroutineRunnerMB? runner;

	public Reference<IApplicable<CoroutineRunnerMB>>[] apply
		= new Reference<IApplicable<CoroutineRunnerMB>>[0];

	public void Start() {
		if (this.runner == null) {
			Debug.LogError($"{this}: requires runner to be set");
		}
	}

	public void Apply() {
		foreach (IApplicable<CoroutineRunnerMB> elem in apply.Values()) {
			elem.Apply(this.runner!);
		}
	}

	public void Release() {
		foreach (IApplicable<CoroutineRunnerMB> elem in apply.Values()) {
			elem.Release(this.runner!);
		}
	}
}
