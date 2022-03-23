using UnityEngine;


public interface ILoadout
{
	void SetStanceAnimator(IStanceAnimation stanceAnimator);
	void Equip(Transform slot);
	void UnEquip();
}

public class LoadoutMB : MonoBehaviour, ILoadout
{
	public Transform? weapon;
	public Stance stance;

	private Transform? originalParent;
	private IStanceAnimation? stanceAnimator;

	public void Equip(Transform slot) {
		this.UpdateStance(1f);

		if (this.weapon == null) {
			return;
		}

		this.originalParent = this.weapon.parent;
		this.UpdateWeapon(slot, true);
	}

	public void UnEquip() {
		this.UpdateStance(0f);
		this.UpdateWeapon(this.originalParent, false);
	}

	private void UpdateStance(float weight) {
		if (this.stanceAnimator == null) {
			return;
		}
		this.stanceAnimator.Set(this.stance, weight);
	}

	private void UpdateWeapon(Transform? parent, bool active) {
		if (this.weapon == null) {
			return;
		}

		this.weapon.parent = null;
		this.weapon.gameObject.SetActive(active);

		if (parent == null) {
			return;
		}

		this.weapon.position = parent.position;
		this.weapon.rotation = parent.rotation;
		this.weapon.parent = parent;
	}

	public void SetStanceAnimator(IStanceAnimation stanceAnimator) {
		this.stanceAnimator = stanceAnimator;
	}
}
