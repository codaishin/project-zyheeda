using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EquipmentMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator GetItem()
	{
		var equipment = new GameObject("equipment").AddComponent<EquipmentMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();
		equipment.records = new Record<EquipmentSlot, ItemMB>[] {
			new Record<EquipmentSlot, ItemMB> { key = EquipmentSlot.OffHand, value = item },
		};

		yield return new WaitForEndOfFrame();

		Assert.AreSame(item, equipment.Equipment[EquipmentSlot.OffHand]);
	}

	[UnityTest]
	public IEnumerator SetItem()
	{
		var equipment = new GameObject("equipment").AddComponent<EquipmentMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();
		equipment.records = new Record<EquipmentSlot, ItemMB>[0];

		yield return new WaitForEndOfFrame();

		equipment.Equipment[EquipmentSlot.OffHand] = item;

		CollectionAssert.AreEqual(
			new (EquipmentSlot, ItemMB)[] { (EquipmentSlot.OffHand, item) },
			equipment.records.Select(r => (r.key, r.value))
		);
	}

		[UnityTest]
	public IEnumerator SetItemSetSheet()
	{
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var equipment = new GameObject("equipment").AddComponent<EquipmentMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();
		equipment.records = new Record<EquipmentSlot, ItemMB>[0];

		yield return new WaitForEndOfFrame();

		equipment.sheet = sheet;
		equipment.Equipment[default] = item;

		Assert.AreSame(sheet, equipment.records[0].value.Sheet);
	}

	[UnityTest]
	public IEnumerator OnValidateDuplicates()
	{
		var equipment = new GameObject("equipment").AddComponent<EquipmentMB>();
		equipment.records = new Record<EquipmentSlot, ItemMB>[] {
			new Record<EquipmentSlot, ItemMB> { key = EquipmentSlot.OffHand },
			new Record<EquipmentSlot, ItemMB> { key = EquipmentSlot.MainHand },
			new Record<EquipmentSlot, ItemMB> { key = EquipmentSlot.OffHand },
		};

		yield return new WaitForEndOfFrame();

		equipment.OnValidate();

		CollectionAssert.AreEqual(
			new string[] { $"{EquipmentSlot.OffHand}", $"{EquipmentSlot.MainHand}", "__duplicate__" },
			equipment.records.Select(r => r.name)
		);
	}

	[UnityTest]
	public IEnumerator OnValidateSetSheet()
	{
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var equipment = new GameObject("equipment").AddComponent<EquipmentMB>();
		equipment.records = new Record<EquipmentSlot, ItemMB>[] {
			new Record<EquipmentSlot, ItemMB> {
				key = EquipmentSlot.OffHand,
				value = new GameObject("itemA").AddComponent<ItemMB>(),
			},
			new Record<EquipmentSlot, ItemMB> {
				key = EquipmentSlot.MainHand,
				value = new GameObject("itemB").AddComponent<ItemMB>(),
			},
		};

		yield return new WaitForEndOfFrame();

		equipment.sheet = sheet;
		equipment.OnValidate();

		CollectionAssert.AreEqual(
			new CharacterSheetMB[] { sheet, sheet },
			equipment.records.Select(r => r.value.Sheet)
		);
	}
}
