using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StatsTests : TestCollection
{
	[Test]
	public void Add()
	{
		var a = new Stats { body = 10, mind = 20, spirit = 44 };
		var b = new Stats { body = 11, mind = -5, spirit = 42 };
		var c = a + b;

		Assert.AreEqual(
			(21, 15, 86),
			(c.body, c.mind, c.spirit)
		);
	}
}
