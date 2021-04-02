using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterSheetMBTests : TestCollection
{
	private class MockClass {}

	[Test]
	public void UseSectionFalse()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		Action fallback = () => {};

		Assert.AreEqual(fallback, sheet.UseSection((ref MockClass _) => {}, fallback));
	}

	[Test]
	public void UseHealthSection()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		sheet.health.hp = 42;

		var exec = sheet.UseSection((ref Health h) => h.hp = 5, default);
		exec();

		Assert.AreEqual(5f, sheet.health.hp);
	}

	[UnityTest]
	public IEnumerator UseResistanceSection()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var resistance = sheet.GetComponent<ResistanceMB>();
		resistance.records = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Physical, value = 44f },
		};
		var exec = sheet.UseSection((ref Resistance r) => r[EffectTag.Physical] = 2f, default);

		yield return new WaitForFixedUpdate();

		exec();

		var record = resistance.records[0];
		Assert.AreEqual((EffectTag.Physical, 2f), (record.key, record.value));
	}

	[UnityTest]
	public IEnumerator NoResistanceBeforeStart()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var resistance = sheet.GetComponent<ResistanceMB>();
		resistance.records = new Record<EffectTag, float>[0];
		var exec = sheet.UseSection((ref Resistance r) => r[EffectTag.Physical] = 2f, default);

		Assert.Throws<NullReferenceException>(() => exec());
		yield break;
	}

	[UnityTest]
	public IEnumerator UseEffectRunnerMBSection()
	{
		var got = default(EffectRunnerMB);
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var runner = sheet.GetComponent<EffectRunnerMB>();
		var exec = sheet.UseSection((ref EffectRunnerMB r) => got = r, () => Assert.Fail("no runner"));

		yield return new WaitForFixedUpdate();

		exec();

		Assert.AreSame(runner, got);
	}
}
