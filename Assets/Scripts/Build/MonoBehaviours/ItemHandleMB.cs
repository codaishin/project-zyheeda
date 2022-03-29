using UnityEngine;

public interface IEquipable
{
	void Equip(IAnimationStance animator, Transform slot);
	void UnEquip();
}

public interface IItem : IEquipable, IInstructions { }

public class ItemHandleMB : BaseInstructionsMB<ItemAction>, IItem
{
	public Animation.Stance idleStance;

	private IAnimationStance? animateStance;
	private Transform? originalParent;

	public void Equip(IAnimationStance animator, Transform slot) {
		this.originalParent = this.transform.parent;

		this.animateStance = animator;

		this.transform.parent = null;
		this.transform.position = slot.position;
		this.transform.rotation = slot.rotation;
		this.transform.parent = slot;

		this.animateStance.Set(this.idleStance, true);

		this.gameObject.SetActive(true);
	}

	public void UnEquip() {
		this.transform.parent = this.originalParent;
		this.animateStance!.Set(this.idleStance, false);

		this.gameObject.SetActive(false);
	}
}
