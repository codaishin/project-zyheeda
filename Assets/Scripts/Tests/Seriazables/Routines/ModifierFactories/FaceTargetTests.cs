using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class FaceTargetTests : TestCollection
	{
		[UnityTest]
		public IEnumerator beginFaceTarget() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData { target = target.transform };

			yield return new WaitForEndOfFrame();

			var modifiers = plugin.GetModifierFnFor(agent)!;
			var begin = modifiers(data).begin!;

			target.transform.position = Vector3.left;

			begin();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator beginFaceTargetDelayedSet() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData();

			yield return new WaitForEndOfFrame();

			var modifiers = plugin.GetModifierFnFor(agent)!;
			var begin = modifiers(data).begin!;

			target.transform.position = Vector3.left;

			data.target = target.transform;
			begin();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator beginFaceTargetRelative() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData { target = target.transform };

			yield return new WaitForEndOfFrame();

			var modifiers = plugin.GetModifierFnFor(agent)!;
			var begin = modifiers(data).begin!;

			target.transform.position = Vector3.up + Vector3.right;
			agent.transform.position = Vector3.left;

			begin();

			Tools.AssertEqual(Vector3.right, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator beginDoesntThrowWhenNoTargetSet() {
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData();

			yield return new WaitForEndOfFrame();

			var modifiers = plugin.GetModifierFnFor(agent)!;
			var begin = modifiers(data).begin!;

			Assert.DoesNotThrow(() => begin());
		}
	}
}
