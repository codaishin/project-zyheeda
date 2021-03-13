using System.Linq;
using NUnit.Framework;

public class ResistanceDataExtensionsTests : TestCollection
{
	[Test]
	public void ConsolidateUnchanged()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
		};
		CollectionAssert.AreEqual(
			data.Select(d => (d.tag, d.value)),
			data.Consolidate().Select(d => (d.tag, d.value))
		);
	}

	[Test]
	public void ConsolidateSetsNames()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
		};
		CollectionAssert.AreEqual(
			new string[] { EffectTag.Heat.ToString(), EffectTag.Physical.ToString() },
			data.Consolidate().Select(d => d.name)
		);
	}

	[Test]
	public void ConsolidateDuplicateItem()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
		};

		CollectionAssert.AreEqual(
			new EffectTag[] { EffectTag.Physical, EffectTag.Heat, (EffectTag)(-1) },
			data.Consolidate().Select(d => d.tag)
		);
	}

	[Test]
	public void ConsolidateDuplicateName()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Physical, value = 0.1f },
		};

		CollectionAssert.AreEqual(
			new string[] { EffectTag.Physical.ToString(), EffectTag.Heat.ToString(), "__unset__" },
			data.Consolidate().Select(d => d.name)
		);
	}
}
