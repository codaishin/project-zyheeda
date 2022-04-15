using NUnit.Framework;
using UnityEngine;

namespace Routines
{
	public class AllignForwardWithMovementTests : TestCollection
	{
		[Test]
		public void AllignForwardUp() {
			var agent = new GameObject();
			var plugin = new AllignForwardWithMovement();
			var modifiers = plugin.GetModifierFnFor(agent)(new Data());

			modifiers.update?.Invoke();

			agent.transform.position = Vector3.up;

			modifiers.update?.Invoke();

			Tools.AssertEqual(Vector3.up, agent.transform.forward);
		}

		[Test]
		public void AllignForwardLeft() {
			var agent = new GameObject();
			var plugin = new AllignForwardWithMovement();
			var modifiers = plugin.GetModifierFnFor(agent)(new Data());

			modifiers.update?.Invoke();

			agent.transform.position = Vector3.left;

			modifiers.update?.Invoke();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[Test]
		public void AllignForwardWithMovement() {
			var agent = new GameObject();
			var plugin = new AllignForwardWithMovement();
			var modifiers = plugin.GetModifierFnFor(agent)(new Data());

			agent.transform.position = Vector3.up;

			modifiers.update?.Invoke();

			agent.transform.position = Vector3.up + Vector3.right;

			modifiers.update?.Invoke();

			Tools.AssertEqual(Vector3.right, agent.transform.forward);
		}
	}
}
