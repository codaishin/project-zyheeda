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

		public void Run(InputAction.CallbackContext context) =>
			this.performed?.Invoke(context);
		public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
			this.performed += listener;
		public TValue ReadValue<TValue>() where TValue : struct =>
			throw new NotImplementedException();
	}

	class MockInputActionSO : BaseInputActionSO<MockInputAction>
	{
		MockInputAction inputAction = new();

		protected override MockInputAction Wrap(InputAction action) =>
			this.inputAction;

		protected override void OnEnable() {
			this.config = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
			base.OnEnable();
		}
	}

	class MockInputListenerMB : BaseInputListenerMB<MockInputAction> { }

	[UnityTest]
	public IEnumerator CallOnInput() {
		var called = 0;
		var so = ScriptableObject.CreateInstance<MockInputActionSO>();
		var mb = new GameObject("obj").AddComponent<MockInputListenerMB>();
		mb.inputActionSO = so;

		yield return new WaitForEndOfFrame();

		mb.OnInput.AddListener(_ => ++called);
		so.InputAction.Run(new());

		Assert.AreEqual(1, called);
	}
}
