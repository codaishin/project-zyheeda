using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputLayerSwitchMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator EnableMovement() {
		var layerSwitch = new GameObject("layerSwitch").AddComponent<InputLayerSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
		layerSwitch.setTo = ActionMap.Movement;
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.playerInputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Movement.enabled);
	}

	[UnityTest]
	public IEnumerator EnableMouse() {
		var layerSwitch = new GameObject("layerSwitch").AddComponent<InputLayerSwitchMB>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		var configSO = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
		layerSwitch.setTo = ActionMap.Mouse;
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.playerInputConfig = configSO;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		yield return new WaitForEndOfFrame();

		Assert.True(configSO.Config.Mouse.enabled);
	}
}
