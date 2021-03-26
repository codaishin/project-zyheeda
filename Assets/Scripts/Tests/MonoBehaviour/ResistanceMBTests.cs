using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ResistanceMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator GetResistance()
	{
		var resistance = new GameObject("resistance").AddComponent<ResistanceMB>();
		resistance.records = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Heat, value = -0.4f },
		};

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(-0.4f, resistance.Resistance[EffectTag.Heat]);
	}

	[UnityTest]
	public IEnumerator SetResistance()
	{
		var resistance = new GameObject("resistance").AddComponent<ResistanceMB>();
		resistance.records = new Record<EffectTag, float>[0];

		yield return new WaitForFixedUpdate();

		resistance.Resistance[EffectTag.Physical] = 1f;

		CollectionAssert.AreEqual(
			new (EffectTag, float)[] { (EffectTag.Physical, 1f) },
			resistance.records.Select(r => (r.key, r.value))
		);
	}

	[UnityTest]
	public IEnumerator OnValidate()
	{
		var resistance = new GameObject("resistance").AddComponent<ResistanceMB>();
		resistance.records = new Record<EffectTag, float>[] {
			new Record<EffectTag, float>{ key = EffectTag.Heat, value = -0.4f },
			new Record<EffectTag, float>{ key = EffectTag.Heat, value = -0.1f },
		};

		yield return new WaitForFixedUpdate();

		resistance.OnValidate();

		CollectionAssert.AreEqual(
			new string[] { EffectTag.Heat.ToString(), "__duplicate__" },
			resistance.records.Select(r => r.name)
		);
	}
}
