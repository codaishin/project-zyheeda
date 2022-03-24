using UnityEngine;


public interface ILoadout
{
	void SetAnimator(IAnimationStance animator);
	void Equip(Transform slot);
	void UnEquip();
}

public class LoadoutMB : MonoBehaviour, ILoadout
{
	public Reference<IItem> weapon;

	private Transform? originalParent;
	private IAnimationStance? animator;

	public void Equip(Transform slot) {
		this.SetStance(true);

		if (this.weapon.Value == null) {
			return;
		}

		this.originalParent = this.weapon.Value.transform.parent;
		this.UpdateWeapon(slot, true);
	}

	public void UnEquip() {
		this.SetStance(false);
		this.UpdateWeapon(this.originalParent, false);
	}

	private void SetStance(bool value) {
		if (this.animator == null || this.weapon.Value == null) {
			return;
		}
		this.animator.Set(this.weapon.Value.IdleStance, value);
	}

	private void UpdateWeapon(Transform? parent, bool active) {
		if (this.weapon.Value == null) {
			return;
		}

		Transform transform = this.weapon.Value.transform;

		transform.parent = null;
		transform.gameObject.SetActive(active);

		if (parent == null) {
			return;
		}

		transform.position = parent.position;
		transform.rotation = parent.rotation;
		transform.parent = parent;
	}

	public void SetAnimator(IAnimationStance animator) {
		this.animator = animator;
	}
}
