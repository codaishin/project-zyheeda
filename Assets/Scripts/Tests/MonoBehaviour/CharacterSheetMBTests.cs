using System;
using NUnit.Framework;
using UnityEngine;

public class CharacterSheetMBTests : TestCollection
{
	[Test]
	public void AddIntensityStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.True(sheet.gameObject.TryGetComponent(out IntensityManagerMB _));
	}

	[Test]
	public void AddDurationStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.True(sheet.gameObject.TryGetComponent(out DurationManagerMB _));
	}

	[Test]
	public void StackIntensity()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect{ duration = 1f, tag = EffectTag.Heat, stacking = ConditionStacking.Intensity };

		sheet.Add(effect);

		CollectionAssert.AreEqual(
			new Effect[] { effect },
			sheet.GetComponent<IntensityManagerMB>().GetEffects(EffectTag.Heat)
		);
	}

	[Test]
	public void DurationStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect{ duration = 1f, tag = EffectTag.Heat, stacking = ConditionStacking.Duration  };

		sheet.Add(effect);

		CollectionAssert.AreEqual(
			new Effect[] { effect },
			sheet.GetComponent<DurationManagerMB>().GetEffects(EffectTag.Heat)
		);
	}

	[Test]
	public void InvalidStacking()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect{ stacking = (ConditionStacking)(-1) };

		Assert.Throws<ArgumentException>(() => sheet.Add(effect));
	}

	[Test]
	public void InvalidStackingMessage()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect{ stacking = (ConditionStacking)(-1) };

		try {
			sheet.Add(effect);
		} catch (ArgumentException e) {
			Assert.AreEqual("Invalid stacking -1 for obj (CharacterSheetMB)", e.Message);
		}
	}

	private class MockClass {}

	[Test]
	public void UseSectionRequiredFail()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.Throws<ArgumentException>(() => sheet.UseSection<MockClass>((ref MockClass _) => {}, true));
	}

	[Test]
	public void UseSectionRequiredFailMessage()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		try {
			sheet.UseSection<MockClass>((ref MockClass _) => {}, true);
		} catch (ArgumentException e) {
			Assert.AreEqual($"{typeof(MockClass)} is not a valid section for obj (CharacterSheetMB)", e.Message);
		}
	}

	[Test]
	public void UseSectionRequiredFailWithoutTrhow()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.DoesNotThrow(() => sheet.UseSection<MockClass>((ref MockClass _) => {}, false));
	}

	[Test]
	public void UseHealthSection()
	{
		var called = default(Health);
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		sheet.health.hp = 42;

		sheet.UseSection((ref Health h) => called = h, false);

		Assert.AreEqual(42, called.hp);
	}
}
