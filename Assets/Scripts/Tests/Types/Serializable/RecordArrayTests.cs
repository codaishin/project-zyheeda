using System.Linq;
using NUnit.Framework;

public class RecordArrayTests : TestCollection
{
	[Test]
	public void GetEmptyValue() {
		var dict = new RecordArray<int, string>();
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void Records() {
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 2, value = "other two" },
		};
		var dict = new RecordArray<int, string>(records);

		CollectionAssert.AreEqual(records, dict);
	}

	[Test]
	public void SetValue() {
		var dict = new RecordArray<int, string>();

		dict[20] = "Hello";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello") },
			dict.Select(r => (r.key, r.value))
		);
	}

	[Test]
	public void UpdateValue() {
		var dict = new RecordArray<int, string>();

		dict[20] = "Hello";
		dict[20] += ", dict!";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello, dict!") },
			dict.Select(r => (r.key, r.value))
		);
	}

	[Test]
	public void SetNames() {
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 3, value = "three" },
		};
		var dict = new RecordArray<int, string>(records);
		dict.SetNamesFromKeys("duplicate");

		CollectionAssert.AreEqual(
			new string[] { "2", "3" },
			dict.Select(r => r.name)
		);
	}

	[Test]
	public void SetNamesWithDuplicates() {
		var records = new Record<int, string>[] {
			new Record<int, string> { key = 2, value = "two" },
			new Record<int, string> { key = 2, value = "other two" },
			new Record<int, string> { key = 3, value = "three" },
		};
		var dict = new RecordArray<int, string>(records);
		dict.SetNamesFromKeys("_");

		CollectionAssert.AreEqual(
			new string[] { "2", "_", "3" },
			dict.Select(r => r.name)
		);
	}

	[Test]
	public void RunOnAdd() {
		var passed = (string.Empty, 0f);
		var recod = new RecordArray<string, float>();

		recod.OnAdd += (k, v) => passed = (k, v);
		recod["two"] = 2f;

		Assert.AreEqual(("two", 2f), passed);
	}
}
