using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputSOTests : TestCollection
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
	public void HoldSpace()
	{
		var key = KeyCode.Space;
		var mode = KeyState.Hold;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		inputSO.keyStates[key] = mode;

		Assert.True(inputSO.GetKey(key, mode));
	}

	[Test]
	public void HoldSpaceFalse()
	{
		var key = KeyCode.Space;
		var mode = KeyState.Hold;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();

		Assert.False(inputSO.GetKey(key, mode));
	}

	[Test]
	public void DownK()
	{
		var key = KeyCode.K;
		var mode = KeyState.Down;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		inputSO.keyStates[key] = mode;

		Assert.True(inputSO.GetKey(key, mode));
	}

	[Test]
	public void DownKFalse()
	{
		var key = KeyCode.K;
		var mode = KeyState.Down;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();

		Assert.False(inputSO.GetKey(key, mode));
	}

	[Test]
	public void UpE()
	{
		var key = KeyCode.E;
		var mode = KeyState.Up;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		inputSO.keyStates[key] = mode;

		Assert.True(inputSO.GetKey(key, mode));
	}

	[Test]
	public void UpEFalse()
	{
		var key = KeyCode.E;
		var mode = KeyState.Up;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();

		Assert.False(inputSO.GetKey(key, mode));
	}

	[Test]
	public void OddStateWThrows()
	{
		var key = KeyCode.W;
		var mode = (KeyState)(-1);
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();

		Assert.Throws<System.ArgumentException>(() => inputSO.GetKey(key, mode));
	}

	[Test]
	public void OddStateWThrowsMessage()
	{
		var key = KeyCode.W;
		var mode = (KeyState)(-1);
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();

		try {
			inputSO.GetKey(key, mode);
		} catch (System.ArgumentException e) {
			Assert.AreEqual("KeyState \"-1\" not recognised", e.Message);
		}
	}
}
