using System.Collections.Generic;
using NUnit.Framework;

public class ListExtensionsTests : TestCollection
{
	[Test]
	public void RemoveLastTrue() {
		var list = new List<int> { 2, 1, 5 };
		Assert.True(list.RemoveLast());
	}

	[Test]
	public void RemoveLast() {
		var list = new List<int> { 2, 1, 5 };
		list.RemoveLast();
		CollectionAssert.AreEqual(new int[] { 2, 1 }, list);
	}

	[Test]
	public void RemoveLastFalse() {
		var list = new List<int>();
		Assert.False(list.RemoveLast());
	}
}
