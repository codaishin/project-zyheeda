using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILoadoutManager
{
	ILoadout Current { get; }
}

public interface ILoadoutCircle : IApplicable, ILoadoutManager { }

public class LoadoutCircleMB : MonoBehaviour, ILoadoutCircle
{
	public Transform? slot;
	public Reference<IAnimationStance> animator;

	private IEnumerator<IEquipableLoadout> loadout =
		Enumerable
			.Empty<IEquipableLoadout>()
			.GetEnumerator();

	public Reference<IEquipableLoadout>[] loadouts = new Reference<IEquipableLoadout>[0];

	public ILoadout Current => this.loadout.Current;

	private void Start() {
		this.loadout = this.LoadoutIterator();
		this.loadout.MoveNext();
		this.loadout.Current.Equip(this.slot!);

		if (this.animator.Value == null) {
			return;
		}
		foreach (IEquipable loadout in this.loadouts.Values()) {
			loadout.SetAnimator(this.animator.Value);
		}
	}

	private IEnumerator<IEquipableLoadout> LoadoutIterator() {
		while (true) {
			foreach (Reference<IEquipableLoadout> loadout in this.loadouts) {
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
