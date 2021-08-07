using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IEnumerableExtensionTests : TestCollection
{
	private class Mutable { public int value; }

	[Test]
	public void ForEach() {
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
	public void OrEmptyThis() {
		CollectionAssert.AreEqual(
			new int[] { 1, 2, 3, 4 },
			new int[] { 1, 2, 3, 4 }.OrEmpty()
		);
	}

	[Test]
	public void OrEmptyNull() {
		Assert.IsEmpty((null as int[]).OrEmpty());
	}

	[Test]
	public void ConcatValue() {
		var concatenated = new int[] { 1, 2, 3 }.Concat(4);
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, concatenated);
	}

	[Test]
	public void ApplyAllActions() {
		var a = 1;
		var b = "1";

		var actions = new Action[] {
			() => a += 1,
			() => b += "1",
		} as IEnumerable<Action>;
		actions.Apply();

		Assert.AreEqual((2, "11"), (a, b));
	}
}
