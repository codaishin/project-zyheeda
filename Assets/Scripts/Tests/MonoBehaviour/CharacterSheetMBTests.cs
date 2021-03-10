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
		var effect = new Effect{ duration = 1f };

		sheet.Add(effect, EffectTag.Heat, ConditionStacking.Intensity);

		CollectionAssert.AreEqual(
			new Effect[] { effect },
			sheet.GetComponent<IntensityManagerMB>().GetEffects(EffectTag.Heat)
		);
	}

	[Test]
	public void DurationStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect{ duration = 1f };

		sheet.Add(effect, EffectTag.Heat, ConditionStacking.Duration);

		CollectionAssert.AreEqual(
			new Effect[] { effect },
			sheet.GetComponent<DurationManagerMB>().GetEffects(EffectTag.Heat)
		);
	}

	[Test]
	public void InvalidStacking()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect();

		Assert.Throws<ArgumentException>(
			() => sheet.Add(effect, EffectTag.Heat, (ConditionStacking)(-1))
		);
	}

	[Test]
	public void InvalidStackingMessage()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();
		var effect = new Effect();

		try {
			sheet.Add(effect, EffectTag.Heat, (ConditionStacking)(-1));
		} catch (ArgumentException e) {
			Assert.AreEqual("Invalid stacking -1 for obj (CharacterSheetMB)", e.Message);
		}
	}
}
