using UnityEngine;

public interface ILoadout
{
	IItem? Item { get; }
}

public interface IEquipable
{
	void SetAnimator(IAnimationStance animator);
	void Equip(Transform slot);
	void UnEquip();
}

public interface IEquipableLoadout : IEquipable, ILoadout { }

public class LoadoutMB : MonoBehaviour, IEquipableLoadout
{
	public Reference<IComplexItem> item;

	private Transform? originalParent;
	private IAnimationStance? animator;

	public IItem? Item => this.item.Value;

	public void Equip(Transform slot) {
		this.SetStance(true);

		if (this.item.Value == null) {
			return;
		}

		this.originalParent = this.item.Value.transform.parent;
		this.UpdateWeapon(slot, true);
	}

	public void UnEquip() {
		this.SetStance(false);
		this.UpdateWeapon(this.originalParent, false);
	}

	private void SetStance(bool value) {
		if (this.animator == null || this.item.Value == null) {
			return;
		}
		this.animator.Set(this.item.Value.IdleStance, value);
	}

	private void UpdateWeapon(Transform? parent, bool active) {
		if (this.item.Value == null) {
			return;
		}

		Transform transform = this.item.Value.transform;

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
