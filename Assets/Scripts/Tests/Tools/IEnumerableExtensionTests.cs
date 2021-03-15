using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IEnumerableExtensionTests : TestCollection
{
	private class Mutable { public int value; }

	[Test]
	public void ForEach()
	{
		var mutables = new Mutable[] {
			new Mutable { value = 1 },
			new Mutable { value = 2 },
			new Mutable { value = 3 },
		};

		mutables.Select(m => m).ForEach(m => m.value += 1);

		CollectionAssert.AreEqual(
			new int[] { 2, 3, 4 },
			mutables.Select(m => m.value)
		);
	}

	[Test]
	public void OrEmptyThis()
	{
		CollectionAssert.AreEqual(
			new int[] { 1, 2, 3, 4 },
			new int[] { 1, 2, 3, 4 }.OrEmpty()
		);
	}

	[Test]
	public void OrEmptyNull()
	{
		Assert.IsEmpty((null as int[]).OrEmpty());
	}
}
