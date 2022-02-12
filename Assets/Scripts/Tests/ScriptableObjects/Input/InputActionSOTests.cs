using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class InputActionSOTests
{
	class MockInputAction : IInputAction
	{
		public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
			throw new NotImplementedException();
		public TValue ReadValue<TValue>() where TValue : struct =>
			throw new NotImplementedException();
	}

	class MockInputActionSO : BaseInputActionSO<MockInputAction>
	{
		public InputAction? actionParameter;
		public MockInputAction mockWrapper = new();

		protected override MockInputAction Wrap(InputAction action) {
			this.actionParameter = action;
			return mockWrapper;
		}

		protected override void OnEnable() {
			this.config = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
			base.OnEnable();
		}
	}

	class WalkInputActionSO : MockInputActionSO
	{
		public WalkInputActionSO() : base() =>
			this.inputOption = InputOption.Walk;
	}

	class RunInputActionSO : MockInputActionSO
	{
		public RunInputActionSO() : base() =>
			this.inputOption = InputOption.Run;
	}

	class MousePositionInputActionSO : MockInputActionSO
	{
		public MousePositionInputActionSO() : base() =>
			this.inputOption = InputOption.MousePosition;
	}

	[UnityTest]
	public IEnumerator GetWalk() {
		var so = ScriptableObject.CreateInstance<WalkInputActionSO>();

		yield return new WaitForEndOfFrame();

		var expected = new PlayerInputConfig().Movement.Walk;

		Assert.AreEqual(expected.ToString(), so.actionParameter!.ToString());
		Assert.AreEqual(so.mockWrapper, so.InputAction);
	}

	[UnityTest]
	public IEnumerator GetRun() {
		var so = ScriptableObject.CreateInstance<RunInputActionSO>();

		yield return new WaitForEndOfFrame();

		var expected = new PlayerInputConfig().Movement.Run;

		Assert.AreEqual(expected.ToString(), so.actionParameter!.ToString());
		Assert.AreEqual(so.mockWrapper, so.InputAction);
	}

	[UnityTest]
	public IEnumerator GetMouse() {
		var so = ScriptableObject.CreateInstance<MousePositionInputActionSO>();

		yield return new WaitForEndOfFrame();

		var expected = new PlayerInputConfig().Mouse.Position;

		Assert.AreEqual(expected.ToString(), so.actionParameter!.ToString());
		Assert.AreEqual(so.mockWrapper, so.InputAction);
	}
}
