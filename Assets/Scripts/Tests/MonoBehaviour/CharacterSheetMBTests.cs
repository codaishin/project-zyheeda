using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

		sheet.Add(effect, EffectTag.Heat, false);

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

		sheet.Add(effect, EffectTag.Heat, true);

		CollectionAssert.AreEqual(
			new Effect[] { effect },
			sheet.GetComponent<DurationManagerMB>().GetEffects(EffectTag.Heat)
		);
	}
}
