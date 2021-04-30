using System.Linq;
using NUnit.Framework;

public class RecordArrayTests : TestCollection
{
	[Test]
	public void GetEmptyValue()
	{
		var dict = new RecordArray<int, string>();
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void Records()
	{
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 2, value = "other two" },
		};
		var dict = new RecordArray<int, string>(records);

		CollectionAssert.AreEqual(records, dict.Records);
	}

	[Test]
	public void SetValue()
	{
		var dict = new RecordArray<int, string>();

		dict[20] = "Hello";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello") },
			dict.Records.Select(r => (r.key, r.value))
		);
	}

	[Test]
	public void UpdateValue()
	{
		var dict = new RecordArray<int, string>();

		dict[20] = "Hello";
		dict[20] += ", dict!";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello, dict!") },
			dict.Records.Select(r => (r.key, r.value))
		);
	}

	[Test]
	public void SetNames()
	{
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 3, value = "three" },
		};
		var dict = new RecordArray<int, string>(records);
		dict.SetNamesFromKeys(default);

		CollectionAssert.AreEqual(new string[] { "2", "3" }, dict.Records.Select(r => r.name));
	}

	[Test]
	public void SetNamesWithDuplicates()
	{
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 2, value = "other two" },
			new Record<int, string> { key = 3, value = "three" },
		};
		var dict = new RecordArray<int, string>(records);
		dict.SetNamesFromKeys("_");

		CollectionAssert.AreEqual(new string[] { "2", "_", "3" }, dict.Records.Select(r => r.name));
	}

	[Test]
	public void SetNamesDoesNotThrowWhenKeyNull()
	{
		var records = new Record<string, string>[] {
			new Record<string, string> { key = null, value = "two" },
		};
		var dict = new RecordArray<string, string>(records);
		Assert.DoesNotThrow(() => dict.SetNamesFromKeys("_"));
	}
}
