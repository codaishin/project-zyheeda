using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ResistanceDataExtensionsTests : TestCollection
{
	[Test]
	public void ConsolidateUnchanged()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
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
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
		};
		CollectionAssert.AreEqual(
			new string[] { "Heat", "Default" },
			data.Consolidate().Select(d => d.name)
		);
	}

	[Test]
	public void ConsolidateDuplicateItem()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
		};

		CollectionAssert.AreEqual(
			new EffectTag[] { EffectTag.Default, EffectTag.Heat, (EffectTag)(-1) },
			data.Consolidate().Select(d => d.tag)
		);
	}

	[Test]
	public void ConsolidateDuplicateName()
	{
		var data = new Resistance.Data[] {
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
			new Resistance.Data{ tag = EffectTag.Heat, value = 0.3f },
			new Resistance.Data{ tag = EffectTag.Default, value = 0.1f },
		};

		CollectionAssert.AreEqual(
			new string[] { "Default", "Heat", "__unset__" },
			data.Consolidate().Select(d => d.name)
		);
	}
}
