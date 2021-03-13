using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ResistanceTests : TestCollection
{
	[Test]
	public void Empty()
	{
		var resistance = new Resistance();
		Assert.AreEqual(0f, resistance[EffectTag.Heat]);
	}

	[Test]
	public void Value()
	{
		var data = new Resistance.Data[] { new Resistance.Data{ tag = EffectTag.Heat, value = 0.4f }};
		var resistance = new Resistance{ data = data };
		Assert.AreEqual(0.4f, resistance[EffectTag.Heat]);
	}
}
