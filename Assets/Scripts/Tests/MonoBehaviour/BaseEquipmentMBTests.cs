using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEquipmentMBTests : TestCollection
{
	private class MockEquipment : IRecordArray<EquipmentSlot, ItemMB>, ISimpleDict<EquipmentSlot, ItemMB>
	{
		public static MockEquipment Current { get; private set; }

		public Action<string> setNames = _ => { };
		public Action<ItemMB> set = _ => { };
		public Func<EquipmentSlot, ItemMB> get = _ => default;
		public Record<EquipmentSlot, ItemMB>[] records = new Record<EquipmentSlot, ItemMB>[0];

		public MockEquipment() => MockEquipment.Current = this;

		public ItemMB this[EquipmentSlot key]
		{
			get => this.get(key);
			set => this.set(value);
		}

		public Record<EquipmentSlot, ItemMB>[] Records => this.records;

		public void SetNames(string duplicateLabel) => this.setNames(duplicateLabel);
	}

	private class MockEquipmentMB : BaseEquipmentMB<MockEquipment> {}

	[UnityTest]
	public IEnumerator SetSheetOnStart()
	{
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();

		equipment.sheet = sheet;
		MockEquipment.Current.records = new Record<EquipmentSlot, ItemMB>[] {
			new Record<EquipmentSlot, ItemMB>{ value = item },
		};

		yield return new WaitForEndOfFrame();

		Assert.AreSame(sheet, item.Sheet);
	}

	[UnityTest]
	public IEnumerator SetSheetOnStartDoesNotThrowWhenValueNull()
	{
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();

		equipment.sheet = sheet;
		MockEquipment.Current.records = new Record<EquipmentSlot, ItemMB>[1];

		yield return new WaitForEndOfFrame();

		Assert.Pass();
	}

	[UnityTest]
	public IEnumerator OnValidateDuplicates()
	{
		var called = string.Empty;
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();

		yield return new WaitForEndOfFrame();

		MockEquipment.Current.setNames = d => called = d;
		equipment.OnValidate();

		Assert.AreEqual("__duplicate__", called);
	}

	[UnityTest]
	public IEnumerator OnValidateSetSheet()
	{
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();

		yield return new WaitForEndOfFrame();

		equipment.sheet = sheet;
		MockEquipment.Current.records =  new Record<EquipmentSlot, ItemMB>[] {
			new Record<EquipmentSlot, ItemMB>{ value = item },
		};
		equipment.OnValidate();

		Assert.AreSame(sheet, item.Sheet);
	}

	[UnityTest]
	public IEnumerator OnValidateSetSheetDoesNotThrowWhenValueNull()
	{
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();

		yield return new WaitForEndOfFrame();

		equipment.sheet = sheet;
		MockEquipment.Current.records =  new Record<EquipmentSlot, ItemMB>[1];
		Assert.DoesNotThrow(() => equipment.OnValidate());
	}

	[UnityTest]
	public IEnumerator GetValue()
	{
		var item = new GameObject("item").AddComponent<ItemMB>();
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();
		MockEquipment.Current.get = k => k switch {
			EquipmentSlot.OffHand => item,
			_ => default,
		};

		yield return new WaitForEndOfFrame();

		Assert.AreSame(item, equipment[EquipmentSlot.OffHand]);
	}

	[UnityTest]
	public IEnumerator SetValue()
	{
		var set = default(ItemMB);
		var item = new GameObject("item").AddComponent<ItemMB>();
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();
		MockEquipment.Current.set = v => set = v;

		yield return new WaitForEndOfFrame();

		equipment[default] = item;

		Assert.AreSame(item, set);
	}

	[UnityTest]
	public IEnumerator SetValueSetSheet()
	{
		var sheet = new GameObject("sheet").AddComponent<CharacterSheetMB>();
		var item = new GameObject("item").AddComponent<ItemMB>();
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();

		yield return new WaitForEndOfFrame();

		equipment.sheet = sheet;
		equipment[default] = item;

		Assert.AreSame(sheet, item.Sheet);
	}

	[UnityTest]
	public IEnumerator SetValueDoesNotThrowForNullItem()
	{
		var equipment = new GameObject("equipment").AddComponent<MockEquipmentMB>();

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => equipment[default] = null);
	}
}
