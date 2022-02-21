using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputLayerSwitchMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator EnableMovement() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputLayerSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] { InputEnum.Map.Movement };
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Movement.enabled);
	}

	[UnityTest]
	public IEnumerator EnableMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputLayerSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] { InputEnum.Map.Mouse };
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Mouse.enabled);
	}

	[UnityTest]
	public IEnumerator EnableMovementAndMouse() {
		var layerSwitch = new GameObject("layerSwitch")
			.AddComponent<InputLayerSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<InputConfigSO>();
		layerSwitch.enable = new InputEnum.Map[] {
			InputEnum.Map.Mouse,
			InputEnum.Map.Movement
		};
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.inputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(true, true),
			(configSO.Config.Mouse.enabled, configSO.Config.Movement.enabled)
		);
	}
}
