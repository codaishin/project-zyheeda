using System;
using System.Linq;
using NUnit.Framework;

public class IDictRecordExtensionsTests : TestCollection
{
	[Test]
	public void ConsolidateUnchanged()
	{
		var data = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Heat, value = 0.3f },
			new Record<EffectTag, float>{ key = EffectTag.Physical, value = 0.1f },
		};
		CollectionAssert.AreEqual(
			data.Select(d => (d.key, d.value)),
			data.Consolidate<EffectTag, float>().Select(d => (d.key, d.value))
		);
	}

	[Test]
	public void ConsolidateDuplicateItem()
	{
		var data = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Physical, value = 0.1f },
			new Record<EffectTag, float>{ key = EffectTag.Heat, value = 0.3f },
			new Record<EffectTag, float>{ key = EffectTag.Physical, value = 0.1f },
		};

		CollectionAssert.AreEqual(
			new string[] { EffectTag.Physical.ToString(), EffectTag.Heat.ToString(), "__duplicate__" },
			data.Consolidate<EffectTag, float>().Select(r => r.name)
		);
	}

	[Test]
	public void AddOrUpdateUpdate()
	{
		var data = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Physical, value =0.1f },
		};
		var record = new Record<EffectTag, float>{ key = EffectTag.Physical, value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.2f ),
			},
			data
				.AddOrUpdate<EffectTag, float>(record)
				.Select(d => (d.key, d.value))
		);
	}

	[Test]
	public void AddOrUpdateUpdateOnlyHit()
	{
		var data = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key =EffectTag.Physical, value = 0.1f },
			new Record<EffectTag, float>{ key =EffectTag.Heat, value = 0.1f },
		};
		var record = new Record<EffectTag, float>{ key = EffectTag.Physical, value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.2f ),
				(EffectTag.Heat, 0.1f ),
			},
			data
				.AddOrUpdate<EffectTag, float>(record)
				.Select(d => (d.key, d.value))
		);
	}

	[Test]
	public void AddOrUpdateAdd()
	{
		var data = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key =EffectTag.Physical, value = 0.1f },
		};
		var record = new Record<EffectTag, float>{ key = EffectTag.Heat, value = 0.2f };
		CollectionAssert.AreEqual(
			new (EffectTag, float)[] {
				(EffectTag.Physical, 0.1f ),
				(EffectTag.Heat, 0.2f ),
			},
			data
				.AddOrUpdate<EffectTag, float>(record)
				.Select(d => (d.key, d.value))
		);
	}
}
