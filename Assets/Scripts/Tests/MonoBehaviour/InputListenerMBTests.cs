using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputListenerMBTests : TestCollection
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
		var inputListenerMB = new GameObject("obj").AddComponent<InputListenerMB>();

		inputSO.keyStates[key] = state;
		eventSO.Listeners += () => ++called;
		inputListenerMB.keyCode = key;
		inputListenerMB.keyState = state;
		inputListenerMB.events = new EventSO[] { eventSO };
		inputListenerMB.inputSO = inputSO;

		inputListenerMB.Listen();

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
		var inputListenerMB = new GameObject("obj").AddComponent<InputListenerMB>();

		eventSO.Listeners += () => ++called;
		inputListenerMB.keyCode = key;
		inputListenerMB.keyState = state;
		inputListenerMB.events = new EventSO[] { eventSO };
		inputListenerMB.inputSO = inputSO;

		inputListenerMB.Listen();

		Assert.AreEqual(0, called);
	}
}
