using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FaceTargetPluginSOTests : TestCollection
{
	[UnityTest]
	public IEnumerator OnBeginFaceTarget() {
		var target = new GameObject();
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<FaceTargetPluginSO>();
		var data = new TargetPluginData { target = target.transform };

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent)!;
		var onBegin = callbacks(data).onBegin!;

		target.transform.position = Vector3.left;

		onBegin();

		Tools.AssertEqual(Vector3.left, agent.transform.forward);
	}

	[UnityTest]
	public IEnumerator OnBeginFaceTargetRelative() {
		var target = new GameObject();
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<FaceTargetPluginSO>();
		var data = new TargetPluginData { target = target.transform };

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent)!;
		var onBegin = callbacks(data).onBegin!;

		target.transform.position = Vector3.up + Vector3.right;
		agent.transform.position = Vector3.left;

		onBegin();

		Tools.AssertEqual(Vector3.right, agent.transform.forward);
	}

	[UnityTest]
	public IEnumerator OnBeginDoesntThrowWhenNoTargetSet() {
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<FaceTargetPluginSO>();
		var data = new TargetPluginData();

		yield return new WaitForEndOfFrame();

		var callbacks = plugin.GetCallbacks(agent)!;
		var onBegin = callbacks(data).onBegin!;

		Assert.DoesNotThrow(() => onBegin());
	}
}
