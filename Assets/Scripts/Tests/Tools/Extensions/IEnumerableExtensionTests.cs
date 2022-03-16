using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

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

	interface IMock { }
	class MockMB : MonoBehaviour, IMock { }

	[Test]
	public void UnpackReferenceValues() {
		var objects = new GameObject[] {
			new GameObject("A"),
			new GameObject("B"),
			new GameObject("C"),
			new GameObject("D"),
		};
		var references = objects
			.Select(obj => obj.AddComponent<MockMB>())
			.Select(Reference<IMock>.PointToComponent)
			.ToArray();
		var expected = references
			.Select(r => r.Value!)
			.ToArray();

		CollectionAssert.AreEqual(expected, references.Values());
	}
}
