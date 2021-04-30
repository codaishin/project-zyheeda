using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputLayerSOTests : TestCollection
{
	[Test]
	public void CheckFalseOnLayerMismatch()
	{
		var inputLayerSO = ScriptableObject.CreateInstance<InputLayerSO>();
		inputLayerSO.CurrentInputLayer = InputLayer.SkillTargeting;

		Assert.False(inputLayerSO.Check(InputLayer.Default));
	}

	[Test]
	public void CheckTrueOnLayerMatch()
	{
		var inputLayerSO = ScriptableObject.CreateInstance<InputLayerSO>();
		inputLayerSO.CurrentInputLayer = InputLayer.SkillTargeting;

		Assert.True(inputLayerSO.Check(InputLayer.SkillTargeting));
	}
}
