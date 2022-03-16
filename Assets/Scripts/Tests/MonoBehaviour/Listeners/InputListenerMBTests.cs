using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class InputListenerMBTests : TestCollection
{
	class MockInputConfigSO : BaseInputConfigSO
	{
		public Dictionary<InputEnum.Action, InputAction> actions = new() {
			{
				InputEnum.Action.Walk,
				new InputAction(
					binding: "<Gamepad>/leftTrigger",
					type: InputActionType.Button
				)
			},
			{
				InputEnum.Action.Run,
				new InputAction(
					binding: "<Gamepad>/rightTrigger",
					type: InputActionType.Button
				)
			}
		};

		public override InputAction this[InputEnum.Action action] =>
			this.actions.TryGetValue(action, out InputAction input)
				? input
				: throw new ArgumentException();

		public override InputActionMap this[InputEnum.Map map] =>
			throw new NotImplementedException();
	}

	class MockApplicableMB : MonoBehaviour, IApplicable
	{
		public int called = 0;
		public void Apply() => ++this.called;
		public void Release() {
			throw new NotImplementedException();
		}
	}

	private Gamepad? pad;

	[SetUp]
	public void SetUp() {
		this.pad = InputSystem.AddDevice<Gamepad>();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.pad!);
	}

	[UnityTest]
	public IEnumerator CallOnInput() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		var apply = new GameObject("apply").AddComponent<MockApplicableMB>();
		mb.apply = new Reference<IApplicable>[] {
			Reference<IApplicable>.PointToComponent(apply),
		};
		mb.inputConfigSO = so;
		mb.listenTo = new InputEnum.Action[] { InputEnum.Action.Walk };
		so.actions.Values.ForEach(a => a.Enable());

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, apply.called);
	}

	[UnityTest]
	public IEnumerator CallOnInputRun() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		var apply = new GameObject("apply").AddComponent<MockApplicableMB>();
		mb.apply = new Reference<IApplicable>[] {
			Reference<IApplicable>.PointToComponent(apply),
		};
		mb.inputConfigSO = so;
		mb.listenTo = new InputEnum.Action[] { InputEnum.Action.Run };
		so.actions.Values.ForEach(a => a.Enable());

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { rightTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, apply.called);
	}

	[UnityTest]
	public IEnumerator CallOnInputOnlyOncePerFrame() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		var apply = new GameObject("apply").AddComponent<MockApplicableMB>();
		mb.apply = new Reference<IApplicable>[] {
			Reference<IApplicable>.PointToComponent(apply),
		};
		mb.inputConfigSO = so;
		mb.listenTo = new InputEnum.Action[] { InputEnum.Action.Walk };
		so.actions.Values.ForEach(a => a.Enable());

		yield return new WaitForEndOfFrame();


		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, apply.called);
	}

	[UnityTest]
	public IEnumerator CallOnInputNextFrame() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		var apply = new GameObject("apply").AddComponent<MockApplicableMB>();
		mb.apply = new Reference<IApplicable>[] {
			Reference<IApplicable>.PointToComponent(apply),
		};
		mb.inputConfigSO = so;
		mb.listenTo = new InputEnum.Action[] { InputEnum.Action.Walk };
		so.actions.Values.ForEach(a => a.Enable());

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, apply.called);
	}
}
