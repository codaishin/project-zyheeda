using UnityEngine;

public interface IItemHandle
{
	void Use();
	void StopUsing();
	void Equip<TAnimator>(TAnimator animator, Transform slot)
		where TAnimator :
			IAnimationStance,
			IAnimationStates;
	void UnEquip();
}

public class ItemHandleMB : MonoBehaviour, IItemHandle
{
	public Animation.Stance idleStance;
	public Animation.State activeState;
	private IAnimationStance? animateStance;
	private IAnimationStates? animateStates;
	private Transform? originalParent;

	public void Use() {
		this.animateStates!.Set(this.activeState);
	}

	public void StopUsing() {
		this.animateStates!.Set(Animation.State.Idle);
	}

	public void Equip<TAnimator>(TAnimator animator, Transform slot)
		where TAnimator :
			IAnimationStance,
			IAnimationStates {
		this.originalParent = this.transform.parent;

		this.animateStance = animator;
		this.animateStates = animator;

		this.transform.parent = null;
		this.transform.position = slot.position;
		this.transform.rotation = slot.rotation;
		this.transform.parent = slot;

		this.animateStance.Set(this.idleStance, true);
	}

	public void UnEquip() {
		this.transform.parent = this.originalParent;
		this.animateStance!.Set(this.idleStance, false);
	}
}
