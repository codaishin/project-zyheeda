using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InputItemTests : TestCollection
{
	private class MockInputSO : BaseInputSO
	{
		public Dictionary<KeyCode, KeyState> keyStates =
			new Dictionary<KeyCode, KeyState>();

		protected override bool Get(in KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Hold;
		protected override bool GetDown(in KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Down;
		protected override bool GetUp(in KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Up;
	}

	[Test]
	public void TriggerEvent()
	{
		var called = 0;
		var key = KeyCode.Space;
		var state = KeyState.Down;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		var inputItem = new InputItem();

		inputSO.keyStates[key] = state;
		eventSO.Listeners += () => ++called;
		inputItem.keyCode = key;
		inputItem.keyState = state;
		inputItem.events = new EventSO[] { eventSO };
		inputItem.inputSO = inputSO;

		inputItem.Apply();

		Assert.AreEqual(1, called);
	}

	[Test]
	public void DontTriggerEvent()
	{
		var called = 0;
		var key = KeyCode.Space;
		var state = KeyState.Down;
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		var inputItem = new InputItem();

		eventSO.Listeners += () => ++called;
		inputItem.keyCode = key;
		inputItem.keyState = state;
		inputItem.events = new EventSO[] { eventSO };
		inputItem.inputSO = inputSO;

		inputItem.Apply();

		Assert.AreEqual(0, called);
	}
}
