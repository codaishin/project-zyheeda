using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputActionMapSwitchMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator EnableMovement() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] { InputEnum.Map.Movement };
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Movement.enabled);
	}

	[UnityTest]
	public IEnumerator EnableMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] { InputEnum.Map.Mouse };
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Mouse.enabled);
	}

	[UnityTest]
	public IEnumerator EnableMovementAndMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] {
			InputEnum.Map.Mouse,
			InputEnum.Map.Movement
		};
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(true, true),
			(configSO.Config.Mouse.enabled, configSO.Config.Movement.enabled)
		);
	}
	[UnityTest]
	public IEnumerator DisableMovement() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.disable = new InputEnum.Map[] { InputEnum.Map.Movement };
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		configSO.Config.Movement.Enable();

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.False(configSO.Config.Movement.enabled);
	}

	[UnityTest]
	public IEnumerator DisableMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.disable = new InputEnum.Map[] { InputEnum.Map.Mouse };
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		configSO.Config.Mouse.Enable();

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.False(configSO.Config.Mouse.enabled);
	}

	[UnityTest]
	public IEnumerator DisableMovementAndMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputActionMapSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.disable = new InputEnum.Map[] {
			InputEnum.Map.Mouse,
			InputEnum.Map.Movement
		};
		layerSwitch.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(trigger),
		};
		layerSwitch.inputConfig = configSO;

		configSO.Config.Mouse.Enable();
		configSO.Config.Movement.Enable();

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(false, false),
			(configSO.Config.Mouse.enabled, configSO.Config.Movement.enabled)
		);
	}
}
