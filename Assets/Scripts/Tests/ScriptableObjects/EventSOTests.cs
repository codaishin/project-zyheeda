using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EventSOTests : TestCollection
{
	[Test]
	public void RegisterEvent()
	{
		var called = 0;
		var eventSO = ScriptableObject.CreateInstance<EventSO>();

		eventSO.Callbacks += () => ++called;
		eventSO.Raise();

		Assert.AreEqual(1, called);
	}

	[Test]
	public void RaiseEmptyNoException()
	{
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		Assert.DoesNotThrow(() => eventSO.Raise());
	}
}
