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
	public void IntensityStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.AreSame(sheet.GetComponent<IntensityManagerMB>(), sheet.StackIntensity);
	}

	[Test]
	public void DurationStack()
	{
		var sheet = new GameObject("obj").AddComponent<CharacterSheetMB>();

		Assert.AreSame(sheet.GetComponent<DurationManagerMB>(), sheet.StackDuration);
	}
}
