using UnityEngine;

public interface ILoadout
{
	void Circle();
	void Animate(Animation.State state);
}

public class LoadoutMB : MonoBehaviour, ILoadout
{
	public Transform? slot;
	public Reference<IAnimationStates> animator;
	public Reference<IItemHandle>[] items = new Reference<IItemHandle>[0];

	private int index = 0;

	private void Start() {
		this.items[this.index].Value?.Equip(this.animator.Value!, this.slot!);
	}

	public void Circle() {
		this.items[this.index++].Value?.UnEquip();
		if (this.index >= this.items.Length) {
			this.index = 0;
		}
		this.items[this.index].Value?.Equip(this.animator.Value!, this.slot!);
	}

	public void Animate(Animation.State state) {
		this.items[this.index].Value?.Set(state);
	}
}
