using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseResistanceMBTests : TestCollection
{
	private class MockResistance : RecordArray<EffectTag, float>
	{
		public MockResistance()
			: base() { }
		public MockResistance(params Record<EffectTag, float>[] records)
			: base(records) { }
	}

	private class MockResistanceMB : BaseResistanceMB<MockResistance> { }

	[UnityTest]
	public IEnumerator OnValidateSetNames() {
		var resistanceMB = new GameObject("resistance").AddComponent<MockResistanceMB>();
		resistanceMB.resistance = new MockResistance(
			new Record<EffectTag, float> { key = EffectTag.Heat, value = 33 },
			new Record<EffectTag, float> { key = EffectTag.Heat, value = 22 }
		);

		yield return new WaitForEndOfFrame();

		resistanceMB.OnValidate();

		Assert.AreEqual("__duplicate__", resistanceMB.resistance.ElementAt(1).name);
	}
}
