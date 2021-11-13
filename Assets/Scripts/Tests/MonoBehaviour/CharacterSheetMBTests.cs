using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterSheetMBTests : TestCollection
{
	private class MockClass { }

	[Test]
	public void UseSectionFalse() {
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		Action fallback = () => { };

		Assert.AreEqual(
			fallback,
			sheet.UseSection((ref MockClass _) => { }, fallback)
		);
	}

	[Test]
	public void UseHealthSection() {
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		sheet.health.hp = 42;

		var exec = sheet.UseSection((ref Health h) => h.hp = 5, default);
		exec();

		Assert.AreEqual(5f, sheet.health.hp);
	}

	[UnityTest]
	public IEnumerator UseResistanceSection() {
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var resistance = sheet.GetComponent<ResistanceMB>();

		yield return new WaitForFixedUpdate();

		var exec = sheet.UseSection(
			(ref Resistance r) => r[EffectTag.Physical] = 2f
		);
		exec();

		var record = resistance.resistance.First();
		Assert.AreEqual((EffectTag.Physical, 2f), (record.key, record.value));
	}

	[UnityTest]
	public IEnumerator UseEffectRunnerMBSection() {
		var got = default(IEffectRunner);
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var runner = sheet.GetComponent<EffectRunnerMB>();
		var exec = sheet.UseSection((ref IEffectRunner r) => got = r, () => Assert.Fail("no runner"));

		yield return new WaitForFixedUpdate();

		exec();

		Assert.AreSame(runner, got);
	}

	[UnityTest]
	public IEnumerator UseEquipmentMBSection() {
		var got = default(Equipment);
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var equipment = sheet.GetComponent<EquipmentMB>();
		var exec = sheet.UseSection((ref Equipment e) => got = e, () => Assert.Fail("no equipment"));

		yield return new WaitForFixedUpdate();

		exec();

		Assert.AreSame(equipment.equipment, got);
	}

	[UnityTest]
	public IEnumerator SetEquipmentMBCharacterSheet() {
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var equipment = sheet.GetComponent<EquipmentMB>();

		yield return new WaitForFixedUpdate();

		Assert.AreSame(sheet, equipment.sheet);
	}

	[Test]
	public void ThrowWhenNoFallbackAndNoMatch() {
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		Action fallback = () => { };

		Assert.Throws<KeyNotFoundException>(
			() => sheet.UseSection((ref MockClass _) => { })
		);
	}
}
