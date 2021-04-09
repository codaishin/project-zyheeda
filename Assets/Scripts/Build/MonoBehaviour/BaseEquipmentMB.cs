using System.Linq;
using UnityEngine;

public abstract class BaseEquipmentMB<TEquipment> : MonoBehaviour, ISimpleDict<EquipmentSlot, ItemMB>
	where TEquipment : IRecordArray<EquipmentSlot, ItemMB>, ISimpleDict<EquipmentSlot, ItemMB>, new()
{
	public CharacterSheetMB sheet;

	[SerializeField]
	private TEquipment equipment = new TEquipment();

	public ItemMB this[EquipmentSlot key]
	{
		get => this.equipment[key];
		set {
			this.equipment[key] = value;
			this.AssignSheet(value);
		}
	}

	private void AssignSheet(ItemMB item)
	{
		if (item) {
			item.Sheet = this.sheet;
		}
	}

	private void Start()
	{
		this.equipment.Records
			.Select(r => r.value)
			.ForEach(this.AssignSheet);
	}

	public void OnValidate()
	{
		this.equipment.SetNames(duplicateLabel: "__duplicate__");
		this.equipment.Records
			.Select(r => r.value)
			.ForEach(this.AssignSheet);
	}
}
