using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

public class BaseTargetingSOTests : TestCollection
{
	private class MockSheet {}

	private class MockTargetingSO : BaseTargetingSO<MockSheet>
	{
		public Func<MockSheet, List<MockSheet>, int, IEnumerator<WaitForEndOfFrame>> doSelect =
			(_, __, ___) => Enumerable.Empty<WaitForEndOfFrame>().GetEnumerator();

		protected override
		IEnumerator<WaitForEndOfFrame> DoSelect(MockSheet source, List<MockSheet> targets, int maxCount)
		{
			return this.doSelect(source, targets, maxCount);
		}
	}

	[Test]
	public void RunDoSelect()
	{
		var called = (default(MockSheet), default(List<MockSheet>), 0);
		var source = new MockSheet();
		var list = new List<MockSheet>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		targeting.doSelect = (s, t, c) => {
			called = (s, t, c);
			return Enumerable.Empty<WaitForEndOfFrame>().GetEnumerator();
		};
		targeting.Select(source, list, 42).MoveNext();

		Assert.AreEqual((source, list, 42), called);
	}

	[Test]
	public void RunOnBeginSelect()
	{
		var called = false;
		var trigger = ScriptableObject.CreateInstance<EventSO>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		targeting.onBeginSelect = new UnityEvent();
		targeting.onBeginSelect.AddListener(() => trigger.Raise());
		trigger.Listeners += () => called = true;

		targeting.Select(default, default, default).MoveNext();

		Assert.True(called);
	}

	[Test]
	public void DontRunOnBeginSelectWhenNotIterating()
	{
		var called = false;
		var trigger = ScriptableObject.CreateInstance<EventSO>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		targeting.onBeginSelect = new UnityEvent();
		targeting.onBeginSelect.AddListener(() => trigger.Raise());
		trigger.Listeners += () => called = true;

		targeting.Select(default, default, default);

		Assert.False(called);
	}

	[Test]
	public void RunOnEndSelect()
	{
		var called = false;
		var trigger = ScriptableObject.CreateInstance<EventSO>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		targeting.onEndSelect = new UnityEvent();
		targeting.onEndSelect.AddListener(() => trigger.Raise());
		trigger.Listeners += () => called = true;

		targeting.Select(default, default, default).MoveNext();

		Assert.True(called);
	}

	[Test]
	public void DontRunOnEndSelectBeforeLastIteration()
	{
		var called = false;
		var trigger = ScriptableObject.CreateInstance<EventSO>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		IEnumerator<WaitForEndOfFrame> select() {
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		targeting.doSelect = (_, __, ___) => select();
		targeting.onEndSelect = new UnityEvent();
		targeting.onEndSelect.AddListener(() => trigger.Raise());
		trigger.Listeners += () => called = true;

		var routine = targeting.Select(default, default, default);
		routine.MoveNext();

		Assert.False(called);
	}
}
