using System.Linq;
using UnityEngine;

public class EquipmentMB : MonoBehaviour
{
	public Record<EquipmentSlot, ItemMB>[] records;

	public CharacterSheetMB sheet;

	public Equipment Equipment { get; private set; }

	private void Awake()
	{
		this.Equipment = new Equipment(
			get: () => this.records,
			set: r => {
				this.records = r;
				this.AssignRecordsSheet();
			}
		);
	}

	public void OnValidate()
	{
		if (this.records != null) {
			this.records = this.records.Validate().ToArray();
			this.AssignRecordsSheet();
		}
	}

	private void AssignRecordsSheet()
	{
		this.records.ForEach(r => {
			if (r.value) {
				r.value.Sheet = this.sheet;
			};
		});
	}
}
