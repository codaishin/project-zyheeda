using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseResistanceMBTests : TestCollection
{
	private class MockResistance : RecordArray<EffectTag, float>
	{
		public MockResistance()
			: base() {}
		public MockResistance(params Record<EffectTag, float>[] records)
			: base(records) {}
	}

	private class MockResistanceMB : BaseResistanceMB<MockResistance> {}

	[UnityTest]
	public IEnumerator InitResistanceOnStart()
	{
		var states = (before: false, after: false);
		var resistance = new GameObject("resistance").AddComponent<MockResistanceMB>();

		states.before = resistance.resistance != null;

		yield return new WaitForEndOfFrame();

		states.after = resistance.resistance != null;

		Assert.AreEqual((false, true), states);
	}

	[UnityTest]
	public IEnumerator DontReinitResistanceOnStart()
	{
		var resistance = new MockResistance();
		var resistanceMB = new GameObject("resistance")
			.AddComponent<MockResistanceMB>();

		resistanceMB.resistance = resistance;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(resistance, resistanceMB.resistance);
	}

	[UnityTest]
	public IEnumerator OnValidateSetNames()
	{
		var resistanceMB = new GameObject("resistance").AddComponent<MockResistanceMB>();
		resistanceMB.resistance = new MockResistance(
			new Record<EffectTag, float>{ key = EffectTag.Heat, value= 33 },
			new Record<EffectTag, float>{ key = EffectTag.Heat, value= 22 }
		);

		yield return new WaitForEndOfFrame();

		resistanceMB.OnValidate();

		Assert.AreEqual("__duplicate__", resistanceMB.resistance.ElementAt(1).name);
	}
}
