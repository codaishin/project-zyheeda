using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInspectorDictTests : TestCollection
{
	[Test]
	public void GetEmptyValue()
	{
		var dict = new InspectorDict<int, string>(new List<Record<int, string>>());
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void SetValue()
	{
		var dict = new InspectorDict<int, string>(new List<Record<int, string>>());

		dict[20] = "Hello";

		Assert.AreEqual("Hello", dict[20]);
	}

	[Test]
	public void UpdateValue()
	{
		var dict = new InspectorDict<int, string>(new List<Record<int, string>>());

		dict[20] = "Hello";
		dict[20] += ", dict!";

		Assert.AreEqual("Hello, dict!", dict[20]);
	}


	[Test]
	public void MarkDuplicate()
	{
		var record = new Record<int, float>();
		record.MarkDuplicate(true);

		Assert.AreEqual("__duplicate__", record.name);
	}

	[Test]
	public void MarkDuplicateFalse()
	{
		var record = new Record<int, float>();
		record.key = 23;
		record.MarkDuplicate(false);

		Assert.AreEqual("23", record.name);
	}
}
