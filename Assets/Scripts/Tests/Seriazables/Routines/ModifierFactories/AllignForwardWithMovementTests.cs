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
			var modify = plugin.GetModifierFnFor(agent)(new RoutineData())!;

			modify();

			agent.transform.position = Vector3.up;

			modify();

			Tools.AssertEqual(Vector3.up, agent.transform.forward);
		}

		[Test]
		public void AllignForwardLeft() {
			var agent = new GameObject();
			var plugin = new AllignForwardWithMovement();
			var modify = plugin.GetModifierFnFor(agent)(new RoutineData())!;

			modify();

			agent.transform.position = Vector3.left;

			modify();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[Test]
		public void AllignForwardWithMovement() {
			var agent = new GameObject();
			var plugin = new AllignForwardWithMovement();
			var modify = plugin.GetModifierFnFor(agent)(new RoutineData())!;

			agent.transform.position = Vector3.up;

			modify();

			agent.transform.position = Vector3.up + Vector3.right;

			modify();

			Tools.AssertEqual(Vector3.right, agent.transform.forward);
		}
	}
}
