using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
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
		targeting.Select(source, list, 42);

		Assert.AreEqual((source, list, 42), called);
	}
}
