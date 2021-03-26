using System.Linq;
using NUnit.Framework;

public class RecordsArrayTests : TestCollection
{
	[Test]
	public void GetEmptyValue()
	{
		var records = new Record<int, string>[0];
		var dict = new RecordsArray<int, string>(
			get: () => records,
			set: r => records = r
		);
		Assert.AreEqual(null, dict[20]);
	}

	[Test]
	public void SetValue()
	{
		var records = new Record<int, string>[0];
		var dict = new RecordsArray<int, string>(
			get: () => records,
			set: r => records = r
		);

		dict[20] = "Hello";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello") },
			records.Select(r => (r.key, r.value))
		);
	}

	[Test]
	public void UpdateValue()
	{
		var records = new Record<int, string>[0];
		var dict = new RecordsArray<int, string>(
			get: () => records,
			set: r => records = r
		);

		dict[20] = "Hello";
		dict[20] += ", dict!";

		CollectionAssert.AreEqual(
			new (int, string)[] { (20, "Hello, dict!") },
			records.Select(r => (r.key, r.value))
		);
	}
}
