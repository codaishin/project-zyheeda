using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEquipmentMBTests : TestCollection
{
	private class MockEquipment : RecordArray<EquipmentSlot, ItemMB?>
	{
		public MockEquipment()
			: base() { }
		public MockEquipment(params Record<EquipmentSlot, ItemMB?>[] records)
			: base(records) { }
	}

	private class MockEquipmentMB : BaseEquipmentMB<MockEquipment> { }

	[UnityTest]
	public IEnumerator SetSheetOnStart() {
		var equipmentMB = new GameObject("equipment")
			.AddComponent<MockEquipmentMB>();
		var sheetMB = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var itemMB = new GameObject("item").AddComponent<ItemMB>();

		equipmentMB.sheet = sheetMB;
		equipmentMB.equipment[default] = itemMB;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(sheetMB, itemMB.Sheet);
	}

	[UnityTest]
	public IEnumerator SetSheetOnStartDoesNotThrowWhenValueNull() {
		var sheetMB = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var itemMB = new GameObject("item").AddComponent<ItemMB>();
		var equipmentMB = new GameObject("equipment")
			.AddComponent<MockEquipmentMB>();

		equipmentMB.sheet = sheetMB;
		equipmentMB.equipment = new MockEquipment(
			new Record<EquipmentSlot, ItemMB?> {
				key = EquipmentSlot.MainHand,
				value = itemMB,
			}
		);

		yield return new WaitForEndOfFrame();

		Assert.Pass();
	}

	[UnityTest]
	public IEnumerator OnValidateDuplicates() {
		var called = string.Empty;
		var sheetMB = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var itemMBA = new GameObject("itemA").AddComponent<ItemMB>();
		var itemMBB = new GameObject("itemB").AddComponent<ItemMB>();
		var equipmentMB = new GameObject("equipment")
			.AddComponent<MockEquipmentMB>();

		equipmentMB.sheet = sheetMB;

		yield return new WaitForEndOfFrame();

		equipmentMB.equipment = new MockEquipment(
			new Record<EquipmentSlot, ItemMB?> {
				key = EquipmentSlot.MainHand,
				value = itemMBA
			},
			new Record<EquipmentSlot, ItemMB?> {
				key = EquipmentSlot.MainHand,
				value = itemMBB
			}
		);

		equipmentMB.OnValidate();

		Assert.AreEqual("__duplicate__", equipmentMB.equipment.ElementAt(1).name);
	}

	[UnityTest]
	public IEnumerator OnValidateSetSheet() {
		var sheetMB = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var itemMB = new GameObject("item").AddComponent<ItemMB>();
		var equipmentMB = new GameObject("equipment")
			.AddComponent<MockEquipmentMB>();

		yield return new WaitForEndOfFrame();

		equipmentMB.sheet = sheetMB;
		equipmentMB.equipment = new MockEquipment(
			new Record<EquipmentSlot, ItemMB?> { value = itemMB }
		);

		equipmentMB.OnValidate();

		Assert.AreSame(sheetMB, equipmentMB.equipment.First().value?.Sheet);
	}

	[UnityTest]
	public IEnumerator SetSheetOnAdd() {
		var equipmentMB = new GameObject("equipment")
			.AddComponent<MockEquipmentMB>();
		var sheetMB = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var itemMB = new GameObject("item").AddComponent<ItemMB>();

		yield return new WaitForEndOfFrame();

		equipmentMB.sheet = sheetMB;
		equipmentMB.equipment[default] = itemMB;
		Assert.AreEqual(sheetMB, itemMB.Sheet);
	}
}
