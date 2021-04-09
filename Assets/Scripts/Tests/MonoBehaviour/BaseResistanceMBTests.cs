using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseResistanceMBTests : TestCollection
{
	private class MockResistance : IRecordArray<EffectTag, float>
	{
		public Record<EffectTag, float>[] records = new Record<EffectTag, float>[0];
		public Action<string> setNames = _ => {};

		public Record<EffectTag, float>[] Records => this.records;
		public void SetNamesFromKeys(string duplicateLabel) => this.setNames(duplicateLabel);
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
		var mock = new MockResistance();
		var resistance = new GameObject("resistance").AddComponent<MockResistanceMB>();

		resistance.resistance = mock;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(mock, resistance.resistance);
	}

	[UnityTest]
	public IEnumerator OnValidateSetNames()
	{
		var called = string.Empty;
		var resistance = new GameObject("resistance").AddComponent<MockResistanceMB>();

		yield return new WaitForEndOfFrame();

		resistance.resistance.setNames = d => called = d;
		resistance.OnValidate();

		Assert.AreEqual("__duplicate__", called);
	}
}
