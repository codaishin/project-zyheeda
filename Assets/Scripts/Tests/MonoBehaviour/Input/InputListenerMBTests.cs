using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class InputListenerMBTests : TestCollection
{
	class MockInputAction : IInputAction
	{
		public event Action<InputAction.CallbackContext>? performed;

		public void Run(InputAction.CallbackContext context) {
			this.performed?.Invoke(context);
		}
		public void AddOnCanceled(Action<InputAction.CallbackContext> listener) =>
			throw new NotImplementedException();
		public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
			this.performed += listener;
		public void AddOnStarted(Action<InputAction.CallbackContext> listener) =>
			throw new NotImplementedException();
		public TValue ReadValue<TValue>() where TValue : struct =>
			throw new NotImplementedException();
	}

	class MockInputListenerMB : BaseInputListenerMB<MockInputAction>
	{
		public InputAction? actionParameter;
		public MockInputAction mockWrapper = new();

		protected override MockInputAction GetWrapper(InputAction action) {
			this.actionParameter = action;
			return this.mockWrapper;
		}
	}

	[UnityTest]
	public IEnumerator WalkAction() {
		var mb = new GameObject("obj").AddComponent<MockInputListenerMB>();
		mb.input = InputOption.Walk;

		yield return new WaitForEndOfFrame();

		var walk = new PlayerInputConfig().Movement.Walk;

		Assert.AreEqual(walk.ToString(), mb.actionParameter!.ToString());
	}

	[UnityTest]
	public IEnumerator CallOnInput() {
		var called = 0;
		var mb = new GameObject("obj").AddComponent<MockInputListenerMB>();
		mb.input = InputOption.Walk;

		yield return new WaitForEndOfFrame();

		mb.OnInput.AddListener(_ => ++called);
		mb.mockWrapper.Run(new());

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator RunAction() {
		var mb = new GameObject("obj").AddComponent<MockInputListenerMB>();
		mb.input = InputOption.Run;

		yield return new WaitForEndOfFrame();

		var run = new PlayerInputConfig().Movement.Run;

		Assert.AreEqual(run.ToString(), mb.actionParameter!.ToString());
	}
}
