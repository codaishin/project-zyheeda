using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InputLayerSwitchMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator SetTo() {
		var layerSwitch = new GameObject("layerSwitch").AddComponent<InputLayerSwitchMB>();
		var layer = ScriptableObject.CreateInstance<InputLayerSO>();
		var trigger = ScriptableObject.CreateInstance<ChannelSO>();
		layerSwitch.setTo = InputLayer.UI;
		layerSwitch.listenTo = new ChannelSO[] { trigger };
		layerSwitch.inputLayer = layer;

		yield return new WaitForEndOfFrame();

		trigger.Raise();

		Assert.AreEqual(InputLayer.UI, layer.CurrentInputLayer);
	}
}
