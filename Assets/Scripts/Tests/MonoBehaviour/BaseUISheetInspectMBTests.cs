using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseUISheetInspectMBTests : TestCollection
{
	private class MockSheet : ISections
	{
		public Func<Delegate, Action?, Action> useSection
			= (_, __)
			=> ()
			=> throw new NotImplementedException();

		public Action UseSection<TSection>(
			RefAction<TSection> action,
			Action? fallback
		) => this.useSection(action, fallback);
	}

	private class MockUIInspectHealth : BaseUIInspectorMB<Health>
	{
		public float hp;
		public override void Set(Health health) => this.hp = health.hp;
	}

	private class MockUISheetInspectMB : BaseUISheetInspectMB<MockSheet> { }

	[UnityTest]
	public IEnumerator MonitorHealth() {
		var health = new Health { hp = 42f };
		var sheet = new MockSheet {
			useSection = (d, fb) => d switch {
				RefAction<Health> u => () => u(ref health),
				_ => fb ?? (() => throw new NotImplementedException()),
			}
		};
		var inspect = new GameObject("inspector").AddComponent<MockUISheetInspectMB>();
		var uIHealth = new GameObject("health").AddComponent<MockUIInspectHealth>();
		inspect.uIHealth = uIHealth;

		yield return new WaitForEndOfFrame();

		inspect.SetSheet(sheet);
		inspect.Monitor();

		Assert.AreEqual(42f, uIHealth.hp);
	}

	[UnityTest]
	public IEnumerator Clear() {
		var health = new Health();
		var sheet = new MockSheet {
			useSection = (d, fb) => d switch {
				RefAction<Health> u => () => u(ref health),
				_ => fb ?? (() => throw new NotImplementedException()),
			}
		};
		var inspect = new GameObject("inspector").AddComponent<MockUISheetInspectMB>();
		var uIHealth = new GameObject("health").AddComponent<MockUIInspectHealth>();
		inspect.uIHealth = uIHealth;

		yield return new WaitForEndOfFrame();

		inspect.SetSheet(sheet);
		inspect.Clear();
		inspect.Monitor();
		health.hp = 42f;

		Assert.AreEqual(0f, uIHealth.hp);
	}
}
