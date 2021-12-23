using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AssignCharacterEquipmentMBTests : TestCollection
{
	class MockEquipmentContentMB : BaseHasValueMB<Equipment> { }

	[UnityTest]
	public IEnumerator MapEquipment() {
		var mapper = new GameObject("mapper")
			.AddComponent<AssignCharacterEquipmentMB>();
		var character = new GameObject("character")
			.AddComponent<CharacterSheetMB>();
		var content = new GameObject("equipmentContent")
			.AddComponent<MockEquipmentContentMB>();
		mapper.value = character;
		mapper.content = content;

		yield return new WaitForEndOfFrame();

		var equipmentMB = character.GetComponent<EquipmentMB>();
		mapper.Apply();

		Assert.AreSame(equipmentMB.equipment, content.Value);
	}

	[UnityTest]
	public IEnumerator MapEquipmentOfChild() {
		var mapper = new GameObject("mapper")
			.AddComponent<AssignCharacterEquipmentMB>();
		var characterParent = new GameObject("character parent");
		var character = new GameObject("character")
			.AddComponent<CharacterSheetMB>();
		var content = new GameObject("equipmentContent")
			.AddComponent<MockEquipmentContentMB>();
		character.transform.SetParent(characterParent.transform);
		mapper.value = characterParent;
		mapper.content = content;

		yield return new WaitForEndOfFrame();

		var equipmentMB = character.GetComponent<EquipmentMB>();
		mapper.Apply();

		Assert.AreSame(equipmentMB.equipment, content.Value);
	}
}
