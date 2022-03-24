using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadoutCircleMB : MonoBehaviour, IApplicable
{
	public Transform? slot;
	public Reference<IAnimationStance> animator;

	private IEnumerator<ILoadout> loadout =
		Enumerable
			.Empty<ILoadout>()
			.GetEnumerator();

	public Reference<ILoadout>[] loadouts = new Reference<ILoadout>[0];

	private void Start() {
		this.loadout = this.LoadoutIterator();
		this.loadout.MoveNext();
		this.loadout.Current.Equip(this.slot!);

		if (this.animator.Value == null) {
			return;
		}
		foreach (ILoadout loadout in this.loadouts.Values()) {
			loadout.SetAnimator(this.animator.Value);
		}
	}

	private IEnumerator<ILoadout> LoadoutIterator() {
		while (true) {
			foreach (Reference<ILoadout> loadout in this.loadouts) {
				yield return loadout.Value!;
			}
		}
	}

	public void Apply() {
		this.loadout.Current.UnEquip();
		this.loadout.MoveNext();
		this.loadout.Current.Equip(this.slot!);
	}

	public void Release() { }
}
