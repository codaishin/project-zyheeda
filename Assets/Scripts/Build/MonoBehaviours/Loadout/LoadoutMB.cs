using System.Linq;
using UnityEngine;

public interface ILoadout
{
	void Circle();
}

public class LoadoutMB : MonoBehaviour, ILoadout, IInstructionsTemplate
{
	public Transform? slot;
	public AnimationMB? animator;
	public Reference<IItem>[] items = new Reference<IItem>[0];

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

	public ExternalInstructionsFn GetInstructionsFor(GameObject agent) {
		ExternalInstructionsFn?[] instructions = this.items
			.Select(item => item.Value?.GetInstructionsFor(agent))
			.ToArray();
		return () => instructions[this.index]?.Invoke();
	}
}
