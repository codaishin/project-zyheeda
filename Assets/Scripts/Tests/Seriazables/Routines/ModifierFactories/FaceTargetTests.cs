using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class FaceTargetTests : TestCollection
	{
		[UnityTest]
		public IEnumerator FaceTarget() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData { target = target.transform };

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetPluginFnFor(agent)!;
			var face = modifierFn(data)!;

			target.transform.position = Vector3.left;

			face();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator beginFaceTargetDelayedSet() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData();

			yield return new WaitForEndOfFrame();

			var modifiers = plugin.GetPluginFnFor(agent)!;
			var face = modifiers(data)!;

			target.transform.position = Vector3.left;

			data.target = target.transform;
			face();

			Tools.AssertEqual(Vector3.left, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator FaceTargetRelative() {
			var target = new GameObject();
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData { target = target.transform };

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetPluginFnFor(agent)!;
			var face = modifierFn(data)!;

			target.transform.position = Vector3.up + Vector3.right;
			agent.transform.position = Vector3.left;

			face();

			Tools.AssertEqual(Vector3.right, agent.transform.forward);
		}

		[UnityTest]
		public IEnumerator DoesntThrowWhenNoTargetSet() {
			var agent = new GameObject();
			var plugin = new FaceTarget();
			var data = new TargetData();

			yield return new WaitForEndOfFrame();

			var modifierFn = plugin.GetPluginFnFor(agent)!;
			var face = modifierFn(data)!;

			Assert.DoesNotThrow(() => face());
		}
	}
}
