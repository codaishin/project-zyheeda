using UnityEngine;

public interface IItemHandle : IAnimationStates
{
	void Equip(IAnimationStates animator, Transform slot);
	void UnEquip();
}

public class ItemHandleMB : MonoBehaviour, IItemHandle
{
	private IAnimationStates? animator;
	private Transform? originalParent;

	public void Set(Animation.State state) {
		this.animator!.Set(state);
	}

	public void Equip(IAnimationStates animator, Transform slot) {
		this.originalParent = this.transform.parent;

		this.animator = animator;

		this.transform.parent = null;
		this.transform.position = slot.position;
		this.transform.rotation = slot.rotation;
		this.transform.parent = slot;
	}

	public void UnEquip() {
		this.transform.parent = this.originalParent;
	}
}
