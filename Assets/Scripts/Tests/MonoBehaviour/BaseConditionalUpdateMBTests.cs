using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

public class BaseConditionalUpdateMBTests : TestCollection
{
	class MockCondtional : IConditional<int>
	{
		public Func<int, bool> check = _ => false;

		public bool Check(int value) => this.check(value);
	}

	class MockConditionalUpdateMB : BaseConditionalUpdateMB<int, MockCondtional> {}

	[UnityTest]
	public IEnumerator InitEvent()
	{
		var state = (before: true, after: false);
		var updater = new GameObject("updater").AddComponent<MockConditionalUpdateMB>();
		updater.conditional = new MockCondtional();

		state.before = updater.onUpdate != null;

		yield return new WaitForFixedUpdate();

		state.after = updater.onUpdate != null;

		Assert.AreEqual((false, true), state);
	}

	[UnityTest]
	public IEnumerator DontInitEvent()
	{
		var updater = new GameObject("updater").AddComponent<MockConditionalUpdateMB>();
		var onUpdate = new UnityEvent();
		updater.onUpdate = onUpdate;
		updater.conditional = new MockCondtional();

		yield return new WaitForFixedUpdate();

		Assert.AreSame(onUpdate, updater.onUpdate);
	}

	[UnityTest]
	public IEnumerator InvokeUpdate()
	{
		var called = false;
		var updater = new GameObject("updater").AddComponent<MockConditionalUpdateMB>();
		updater.conditional = new MockCondtional{ check = _ => true };

		yield return new WaitForEndOfFrame();

		updater.onUpdate.AddListener(() => called = true);

		yield return new WaitForEndOfFrame();

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator DontInvokeUpdate()
	{
		var called = false;
		var updater = new GameObject("updater").AddComponent<MockConditionalUpdateMB>();
		updater.conditional = new MockCondtional{ check = _ => false };

		yield return new WaitForEndOfFrame();

		updater.onUpdate.AddListener(() => called = true);

		yield return new WaitForEndOfFrame();

		Assert.False(called);
	}

	[UnityTest]
	public IEnumerator PassCheckValue()
	{
		var called = -1;
		var updater = new GameObject("updater").AddComponent<MockConditionalUpdateMB>();
		updater.value = 42;
		updater.conditional = new MockCondtional{
			check = v => {
				called = v;
				return false;
			}
		};

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(42, called);
	}
}
