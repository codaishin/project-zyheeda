using UnityEngine;

public interface ILoadout
{
	void Circle();
	void Use();
}

public class LoadoutMB : MonoBehaviour, ILoadout
{
	public Transform? slot;
	public AnimationMB? animator;
	public Reference<IItemHandle>[] items = new Reference<IItemHandle>[0];

	private int index = 0;

	private void Start() {
		this.items[this.index].Value?.Equip(this.animator!, this.slot!);
	}

	public void Circle() {
		this.items[this.index++].Value?.UnEquip();
		if (this.index >= this.items.Length) {
			this.index = 0;
		}
		this.items[this.index].Value?.Equip(this.animator!, this.slot!);
	}

	public void Use() {
		this.items[this.index].Value?.Use();
	}
}
