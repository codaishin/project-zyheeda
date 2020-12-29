using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InputItemTests : TestCollection
{
	[Test]
	public void TriggerEvent()
	{
		var called = 0;
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		var inputItem = new InputItem();

		eventSO.Listeners += () => ++called;
		inputItem.events = new EventSO[] { eventSO };

		inputItem.Apply((in KeyCode _, in KeyState __) => true);

		Assert.AreEqual(1, called);
	}

	[Test]
	public void DontTriggerEvent()
	{
		var called = 0;
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		var inputItem = new InputItem();

		eventSO.Listeners += () => ++called;
		inputItem.events = new EventSO[] { eventSO };

		inputItem.Apply((in KeyCode _, in KeyState __) => false);

		Assert.AreEqual(0, called);
	}

	[Test]
	public void ApplyInjectsKeyCode()
	{
		var expected = KeyCode.Space;
		var actual = KeyCode.None;
		var inputItem = new InputItem();

		inputItem.keyCode = expected;
		inputItem.Apply((in KeyCode kC, in KeyState __) => {
			actual = kC;
			return false;
		});

		Assert.AreEqual(expected, actual);
	}

	[Test]
	public void ApplyInjectsKeyState()
	{
		var expected = KeyState.Up;
		var actual = KeyState.Hold;
		var inputItem = new InputItem();

		inputItem.keyState = expected;
		inputItem.Apply((in KeyCode _, in KeyState kS) => {
			actual = kS;
			return false;
		});

		Assert.AreEqual(expected, actual);
	}
}
