using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class DisableDuringCooldownTests : TestCollection
	{
		class MockMB : MonoBehaviour { }

		[UnityTest]
		public IEnumerator Disable() {
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			var template = new DisableDuringCooldown { cooldown = 0.3f };

			yield return new WaitForEndOfFrame();

			var routineFn = template.GetRoutineFnFor(agent)!;

			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.False(agent.activeSelf);
		}

		[UnityTest]
		public IEnumerator NotDisabledBeforeRun() {
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			var template = new DisableDuringCooldown { cooldown = 0.3f };

			yield return new WaitForEndOfFrame();

			var routineFn = template.GetRoutineFnFor(agent)!;
			_ = routineFn();

			yield return new WaitForEndOfFrame();

			Assert.True(agent.activeSelf);
		}

		[UnityTest]
		public IEnumerator EnableAfterCooldown() {
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			var template = new DisableDuringCooldown { cooldown = 0.3f };

			yield return new WaitForEndOfFrame();

			var routineFn = template.GetRoutineFnFor(agent)!;

			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.3f);

			Assert.True(agent.activeSelf);
		}
	}
}
