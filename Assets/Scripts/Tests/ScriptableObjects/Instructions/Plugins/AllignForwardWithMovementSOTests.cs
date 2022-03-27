using NUnit.Framework;
using UnityEngine;

public class AllignForwardWithMovementSOTests : TestCollection
{
	[Test]
	public void AllignForwardUp() {
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<AllignForwardWithMovementSO>();
		var callbacks = plugin.GetCallbacks(agent)(new PluginData());

		callbacks.onAfterYield?.Invoke();

		agent.transform.position = Vector3.up;

		callbacks.onAfterYield?.Invoke();

		Tools.AssertEqual(Vector3.up, agent.transform.forward);
	}

	[Test]
	public void AllignForwardLeft() {
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<AllignForwardWithMovementSO>();
		var callbacks = plugin.GetCallbacks(agent)(new PluginData());

		callbacks.onAfterYield?.Invoke();

		agent.transform.position = Vector3.left;

		callbacks.onAfterYield?.Invoke();

		Tools.AssertEqual(Vector3.left, agent.transform.forward);
	}

	[Test]
	public void AllignForwardWithMovement() {
		var agent = new GameObject();
		var plugin = ScriptableObject.CreateInstance<AllignForwardWithMovementSO>();
		var callbacks = plugin.GetCallbacks(agent)(new PluginData());

		agent.transform.position = Vector3.up;

		callbacks.onAfterYield?.Invoke();

		agent.transform.position = Vector3.up + Vector3.right;

		callbacks.onAfterYield?.Invoke();

		Tools.AssertEqual(Vector3.right, agent.transform.forward);
	}
}
