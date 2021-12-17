using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class KeyInputGroupSOTests
{
	private class MockKeyInputSO : BaseKeyInputSO
	{
		public Func<KeyCode, bool> get = _ => false;
		public Func<KeyCode, bool> getDown = _ => false;
		public Func<KeyCode, bool> getUp = _ => false;
		protected override bool Get(KeyCode key) => this.get(key);
		protected override bool GetDown(KeyCode key) => this.getDown(key);
		protected override bool GetUp(KeyCode key) => this.getUp(key);
	}

	[Test]
	public void KorrektInputSOGetKeyParameters() {
		var called = new List<(KeyCode, KeyState)>();
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		var inputGroupSO = ScriptableObject.CreateInstance<KeyInputGroupSO>();
		var eventU = ScriptableObject.CreateInstance<ChannelSO>();
		var eventD = ScriptableObject.CreateInstance<ChannelSO>();
		var eventH = ScriptableObject.CreateInstance<ChannelSO>();
		inputSO.get = k => {
			called.Add((k, KeyState.Hold));
			return true;
		};
		inputSO.getDown = k => {
			called.Add((k, KeyState.Down));
			return true;
		};
		inputSO.getUp = k => {
			called.Add((k, KeyState.Up));
			return true;
		};
		inputGroupSO.inputSO = inputSO;
		inputGroupSO.input = new RecordArray<ChannelSO, KeyInputItem>(
			new Record<ChannelSO, KeyInputItem> {
				key = eventD,
				value = new KeyInputItem {
					keyCode = KeyCode.D,
					keyState = KeyState.Down
				},
			},
			new Record<ChannelSO, KeyInputItem> {
				key = eventH,
				value = new KeyInputItem {
					keyCode = KeyCode.H,
					keyState = KeyState.Hold
				},
			},
			new Record<ChannelSO, KeyInputItem> {
				key = eventU,
				value = new KeyInputItem {
					keyCode = KeyCode.U,
					keyState = KeyState.Up
				},
			}
		);
		inputGroupSO.Apply();

		CollectionAssert.AreEquivalent(
			new (KeyCode, KeyState)[] {
				(KeyCode.H, KeyState.Hold),
				(KeyCode.D, KeyState.Down),
				(KeyCode.U, KeyState.Up),
			},
			called
		);
	}

	[Test]
	public void AppliesItems() {
		var calledA = 0;
		var calledB = 0;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		var inputGroupSO = ScriptableObject.CreateInstance<KeyInputGroupSO>();
		var eventA = ScriptableObject.CreateInstance<ChannelSO>();
		var eventB = ScriptableObject.CreateInstance<ChannelSO>();

		eventA.Listeners += () => ++calledA;
		eventB.Listeners += () => ++calledB;
		inputSO.getDown = _ => true;
		inputGroupSO.inputSO = inputSO;
		inputGroupSO.input = new RecordArray<ChannelSO, KeyInputItem>(
			new Record<ChannelSO, KeyInputItem> {
				key = eventA,
				value = new KeyInputItem { keyState = KeyState.Down },
			},
			new Record<ChannelSO, KeyInputItem> {
				key = eventB,
				value = new KeyInputItem { keyState = KeyState.Down },
			}
		);
		inputGroupSO.Apply();

		Assert.AreEqual((1, 1), (calledA, calledB));
	}

	[Test]
	public void OnValidateSetsNames() {
		var inputGroupSO = ScriptableObject.CreateInstance<KeyInputGroupSO>();
		var eventA = ScriptableObject.CreateInstance<ChannelSO>();

		eventA.name = "EventA";
		inputGroupSO.input = new RecordArray<ChannelSO, KeyInputItem>(
			new Record<ChannelSO, KeyInputItem> {
				key = eventA,
				value = new KeyInputItem { keyState = KeyState.Down },
			},
			new Record<ChannelSO, KeyInputItem> {
				key = eventA,
				value = new KeyInputItem { keyState = KeyState.Down },
			}
		);
		inputGroupSO.OnValidate();

		var names = inputGroupSO.input.Select(r => r.name);

		CollectionAssert.AreEqual(
			new string[] { "EventA (ChannelSO)", "__duplicate__" },
			names
		);
	}

	[Test]
	public void AppliesOnlyUniqueItems() {
		var called = 0;
		var inputSO = ScriptableObject.CreateInstance<MockKeyInputSO>();
		var inputGroupSO = ScriptableObject.CreateInstance<KeyInputGroupSO>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		eventSO.Listeners += () => ++called;
		inputSO.getDown = _ => true;
		inputGroupSO.inputSO = inputSO;
		inputGroupSO.input = new RecordArray<ChannelSO, KeyInputItem>(
			new Record<ChannelSO, KeyInputItem> {
				key = eventSO,
				value = new KeyInputItem { keyState = KeyState.Down },
			},
			new Record<ChannelSO, KeyInputItem> {
				key = eventSO,
				value = new KeyInputItem { keyState = KeyState.Down },
			}
		);
		inputGroupSO.Apply();

		Assert.AreEqual(1, called);
	}
}
