using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputGroupSOTests
{
	private class MockInputSO : BaseInputSO
	{
		public List<(KeyCode, KeyState)> used = new List<(KeyCode, KeyState)>();

		protected override bool Get(in KeyCode _) => true;
		protected override bool GetDown(in KeyCode _) => true;
		protected override bool GetUp(in KeyCode _) => true;

		public override bool GetKey(in KeyCode code, in KeyState state)
		{
			this.used.Add((code, state));
			return base.GetKey(code, state);
		}
	}

	[Test]
	public void AppliesItems()
	{
		var calledA = 0;
		var calledB = 0;
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		var inputGroupSO = ScriptableObject.CreateInstance<InputGroupSO>();
		var eventA = ScriptableObject.CreateInstance<EventSO>();
		var eventB = ScriptableObject.CreateInstance<EventSO>();

		eventA.Listeners += () => ++calledA;
		eventB.Listeners += () => ++calledB;
		inputGroupSO.inputSO = inputSO;
		inputGroupSO.items = new InputItem[] {
			new InputItem{ eventSO = eventA },
			new InputItem{ eventSO = eventB },
		};
		inputGroupSO.Apply();

		CollectionAssert.AreEqual(
			new int[] { 1, 1 },
			new int[] { calledA, calledB }
		);
	}

	[Test]
	public void InjectsInputSOCallback()
	{
		var inputSO = ScriptableObject.CreateInstance<MockInputSO>();
		var inputGroupSO = ScriptableObject.CreateInstance<InputGroupSO>();
		var eventSO = ScriptableObject.CreateInstance<EventSO>();
		var itemA = new InputItem{
			keyCode = KeyCode.U,
			keyState = KeyState.Up,
			eventSO = eventSO,
		};
		var itemB = new InputItem{
			keyCode = KeyCode.D,
			keyState = KeyState.Down,
			eventSO = eventSO,
		};

		inputGroupSO.inputSO = inputSO;
		inputGroupSO.items = new InputItem[] { itemA, itemB };
		inputGroupSO.Apply();

		CollectionAssert.AreEqual(
			new (KeyCode, KeyState) [] {
				(itemA.keyCode, itemA.keyState),
				(itemB.keyCode, itemB.keyState),
			},
			inputSO.used
		);
	}
}
