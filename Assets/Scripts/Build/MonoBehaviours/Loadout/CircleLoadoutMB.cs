using UnityEngine;

public class CircleLoadoutMB : MonoBehaviour, IApplicable
{
	public Reference<ILoadout> loadout;

	public void Apply() {
		this.loadout.Value!.Circle();
	}

	public void Release() { }
}
