using System.Linq;
using UnityEngine;

public abstract class BaseEquipmentMB<TEquipment> : MonoBehaviour
	where TEquipment :
		IRecordArray<EquipmentSlot, ItemMB>,
		ISimpleDict<EquipmentSlot, ItemMB>,
		new()
{
	public CharacterSheetMB sheet;

	public TEquipment equipment = new TEquipment();

	private void AssignSheet(ItemMB item)
	{
		if (item) {
			item.Sheet = this.sheet;
		}
	}

	private void Start()
	{
		this.equipment
			.Select(r => r.value)
			.ForEach(this.AssignSheet);
	}

	public void OnValidate()
	{
		this.equipment.SetNamesFromKeys(duplicateLabel: "__duplicate__");
		this.equipment
			.Select(r => r.value)
			.ForEach(this.AssignSheet);
	}
}
