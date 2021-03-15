using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ResistanceTests : TestCollection
{
	[Test]
	public void GetEmpty()
	{
		var resistance = new Resistance();
		Assert.AreEqual(0f, resistance[EffectTag.Heat]);
	}

	[Test]
	public void GetValue()
	{
		var data = new Resistance.Data[] { new Resistance.Data{ tag = EffectTag.Heat, value = 0.4f }};
		var resistance = new Resistance{ data = data };
		Assert.AreEqual(0.4f, resistance[EffectTag.Heat]);
	}

	[Test]
	public void SetValue()
	{
		var data = new Resistance.Data[] { new Resistance.Data{ tag = EffectTag.Heat, value = 0.4f }};
		var resistance = new Resistance{ data = data };

		resistance[EffectTag.Heat] = 0.3f;

		Assert.AreEqual(0.3f, resistance[EffectTag.Heat]);
	}

	[Test]
	public void SetNewValue()
	{
		var resistance = new Resistance{ data = new Resistance.Data[0] };

		resistance[EffectTag.Heat] = 0.3f;

		Assert.AreEqual(0.3f, resistance[EffectTag.Heat]);
	}
}
