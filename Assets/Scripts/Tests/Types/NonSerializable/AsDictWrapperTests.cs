using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleDictTests : TestCollection
{
	[Test]
	public void GetEmptyValue()
	{
		var dict = new AsDictWrapper<int, string>(new List<Record<int, string>>());
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void SetValue()
	{
		var dict = new AsDictWrapper<int, string>(new List<Record<int, string>>());

		dict[20] = "Hello";

		Assert.AreEqual("Hello", dict[20]);
	}

	[Test]
	public void UpdateValue()
	{
		var dict = new AsDictWrapper<int, string>(new List<Record<int, string>>());

		dict[20] = "Hello";
		dict[20] += ", dict!";

		Assert.AreEqual("Hello, dict!", dict[20]);
	}
}
