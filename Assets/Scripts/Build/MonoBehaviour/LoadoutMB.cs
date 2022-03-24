using UnityEngine;


public interface ILoadout
{
	void SetAnimator(IAnimationStance animator);
	void Equip(Transform slot);
	void UnEquip();
}

public class LoadoutMB : MonoBehaviour, ILoadout
{
	public Transform? weapon;
	public Animation.Stance stance;

	private Transform? originalParent;
	private IAnimationStance? animator;

	public void Equip(Transform slot) {
		this.SetStance(true);

		if (this.weapon == null) {
			return;
		}

		this.originalParent = this.weapon.parent;
		this.UpdateWeapon(slot, true);
	}

	public void UnEquip() {
		this.SetStance(false);
		this.UpdateWeapon(this.originalParent, false);
	}

	private void SetStance(bool value) {
		if (this.animator == null) {
			return;
		}
		this.animator.Set(this.stance, value);
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

	public void SetAnimator(IAnimationStance animator) {
		this.animator = animator;
	}
}
