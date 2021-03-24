using System;
using System.Linq;
using NUnit.Framework;

public class IDictRecordExtensionsTests : TestCollection
{
	private class MockRecord : IDictRecord<EffectTag, float>
	{
		public EffectTag Key { get; set; }
		public float Value { get; set; }

		public Action<bool> markDuplicate = _ => {};
		public void MarkDuplicate(bool duplicate) => this.markDuplicate(duplicate);
	}

	[Test]
	public void ConsolidateUnchanged()
	{
		var data = new MockRecord[] {
			new MockRecord{ Key = EffectTag.Heat, Value = 0.3f },
			new MockRecord{ Key = EffectTag.Physical, Value = 0.1f },
		};
		CollectionAssert.AreEqual(
			data.Select(d => (d.Key, d.Value)),
			data.Consolidate<MockRecord, EffectTag, float>().Select(d => (d.Key, d.Value))
		);
	}

	[Test]
	public void ConsolidateDuplicateItem()
	{
		var duplicates = (true, true, false);
		var data = new MockRecord[] {
			new MockRecord{ Key = EffectTag.Physical, Value = 0.1f, markDuplicate = d => duplicates.Item1 = d },
			new MockRecord{ Key = EffectTag.Heat, Value = 0.3f, markDuplicate = d => duplicates.Item2 = d },
			new MockRecord{ Key = EffectTag.Physical, Value = 0.1f, markDuplicate = d => duplicates.Item3 = d },
		};

		data.Consolidate<MockRecord, EffectTag, float>().ToArray();

		Assert.AreEqual((false, false, true), duplicates);
	}

	[Test]
	public void AddOrUpdateUpdate()
	{
		var data = new MockRecord[] {
			new MockRecord{ Key =EffectTag.Physical, Value =0.1f },
		};
		var record = new MockRecord{ Key = EffectTag.Physical, Value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.2f ),
			},
			data
				.AddOrUpdate<MockRecord, EffectTag, float>(record)
				.Select(d => (d.Key, d.Value))
		);
	}

	[Test]
	public void AddOrUpdateUpdateOnlyHit()
	{
		var data = new MockRecord[] {
			new MockRecord{ Key =EffectTag.Physical, Value =0.1f },
			new MockRecord{ Key =EffectTag.Heat, Value =0.1f },
		};
		var record = new MockRecord{ Key = EffectTag.Physical, Value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.2f ),
				(EffectTag.Heat, 0.1f ),
			},
			data
				.AddOrUpdate<MockRecord, EffectTag, float>(record)
				.Select(d => (d.Key, d.Value))
		);
	}

	[Test]
	public void AddOrUpdateAdd()
	{
		var data = new MockRecord[] {
			new MockRecord{ Key =EffectTag.Physical, Value = 0.1f },
		};
		var record = new MockRecord{ Key = EffectTag.Heat, Value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.1f ),
				(EffectTag.Heat, 0.2f ),
			},
			data
				.AddOrUpdate<MockRecord, EffectTag, float>(record)
				.Select(d => (d.Key, d.Value))
		);
	}
}
