using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class RecordListExtensionsTests : TestCollection
{
	[Test]
	public void MarkDoublesUnchanged()
	{
		var list = new List<Record<char, string>>{
			new Record<char, string>{ key = 'a' },
			new Record<char, string>{ key = 'b' },
		};

		list.Validate();

		CollectionAssert.AreEqual(new string[] { "a", "b" }, list.Select(r => r.name));
	}

	[Test]
	public void MarkDoubles()
	{
		var list = new List<Record<char, string>>{
			new Record<char, string>{ key = 'a' },
			new Record<char, string>{ key = 'b' },
			new Record<char, string>{ key = 'c' },
			new Record<char, string>{ key = 'b' },
			new Record<char, string>{ key = 'd' },
		};

		list.Validate();

		CollectionAssert.AreEqual(
			new string[] { "a", "b", "c", "__duplicate__", "d" },
			list.Select(r => r.name)
		);
	}
}
