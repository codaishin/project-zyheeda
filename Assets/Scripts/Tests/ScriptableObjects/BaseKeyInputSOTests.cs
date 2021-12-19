using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseKeyInputSOTests : TestCollection
{
	private class MockKeyInputSO : BaseKeyInputSO
	{
		public Dictionary<KeyCode, KeyState> keyStates =
			new Dictionary<KeyCode, KeyState>();

		protected override bool Get(KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Hold;
		protected override bool GetDown(KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Down;
		protected override bool GetUp(KeyCode code) =>
			this.keyStates.TryGetValue(code, out KeyState state) &&
			state == KeyState.Up;
	}

	[Test]
	public void HoldSpace() {
		var key = KeyCode.Space;
		var state = KeyState.Hold;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		inputSO.keyStates[key] = state;

		Assert.True(inputSO.GetKey(key, state));
	}

	[Test]
	public void HoldSpaceFalse() {
		var key = KeyCode.Space;
		var state = KeyState.Hold;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();

		Assert.False(inputSO.GetKey(key, state));
	}

	[Test]
	public void DownK() {
		var key = KeyCode.K;
		var state = KeyState.Down;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		inputSO.keyStates[key] = state;

		Assert.True(inputSO.GetKey(key, state));
	}

	[Test]
	public void DownKFalse() {
		var key = KeyCode.K;
		var state = KeyState.Down;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();

		Assert.False(inputSO.GetKey(key, state));
	}

	[Test]
	public void UpE() {
		var key = KeyCode.E;
		var state = KeyState.Up;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		inputSO.keyStates[key] = state;

		Assert.True(inputSO.GetKey(key, state));
	}

	[Test]
	public void UpEFalse() {
		var key = KeyCode.E;
		var state = KeyState.Up;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();

		Assert.False(inputSO.GetKey(key, state));
	}

	[Test]
	public void OddStateWThrows() {
		var key = KeyCode.W;
		var state = (KeyState)(-1);
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();

		Assert.Throws<System.ArgumentException>(() => inputSO.GetKey(key, state));
	}

	[Test]
	public void OddStateWThrowsMessage() {
		var key = KeyCode.W;
		var state = (KeyState)(-1);
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();

		try {
			inputSO.GetKey(key, state);
		}
		catch (System.ArgumentException e) {
			Assert.AreEqual("KeyState \"-1\" not recognised", e.Message);
		}
	}
}
