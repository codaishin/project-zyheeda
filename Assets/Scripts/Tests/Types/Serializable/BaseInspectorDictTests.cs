using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInspectorDictTests : TestCollection
{
	private class MockDict : BaseInspectorDict<int, string>
	{
		public MockDict(MockDict.Record[] data) : base(data) {}
		public MockDict() : base() {}
	}

	[Test]
	public void GetEmpty()
	{
		var dict = new MockDict();
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void GetValue()
	{
		var data = new MockDict.Record[] { new MockDict.Record{ key = 20, value = "Hello" }};
		var dict = new MockDict(data);
		Assert.AreEqual("Hello", dict[20]);
	}

	[Test]
	public void SetValue()
	{
		var data = new MockDict.Record[] { new MockDict.Record{ key = 20, value = "Hello" }};
		var dict = new MockDict(data);

		dict[20] += ", dict!";

		Assert.AreEqual("Hello, dict!", dict[20]);
	}

	[Test]
	public void SetNewValue()
	{
		var dict = new MockDict();

		dict[20] = "Hello";

		Assert.AreEqual("Hello", dict[20]);
	}

	[Test]
	public void MarkDuplicate()
	{
		var data = new MockDict.Record[1];
		data[0].MarkDuplicate(true);

		Assert.AreEqual("__duplicate__", data[0].name);
	}

	[Test]
	public void MarkDuplicateFalse()
	{
		var data = new MockDict.Record[1];
		data[0].key = 23;
		data[0].MarkDuplicate(false);

		Assert.AreEqual("23", data[0].name);
	}

	[Test]
	public void ConsolidateAfterDeserialization()
	{
		var dict = new MockDict(new MockDict.Record[] {
			new MockDict.Record{ key = 10 },
			new MockDict.Record{ key = 10 },
		});
		dict.OnAfterDeserialize();

		Assert.AreEqual(("10", "__duplicate__"), (dict.Data[0].name, dict.Data[1].name));
	}
}
