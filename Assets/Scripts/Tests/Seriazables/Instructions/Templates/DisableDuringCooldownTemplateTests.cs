using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class DisableDuringCooldownTemplateTests : TestCollection
{
	class MockMB : MonoBehaviour { }

	[UnityTest]
	public IEnumerator Disable() {
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var template = new DisableDuringCooldownTemplate { cooldown = 0.3f };

		yield return new WaitForEndOfFrame();

		var instructions = template.GetInstructionsFor(agent);

		runner.StartCoroutine(instructions()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.False(agent.activeSelf);
	}

	[UnityTest]
	public IEnumerator NotDisabledBeforeRun() {
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var template = new DisableDuringCooldownTemplate { cooldown = 0.3f };

		yield return new WaitForEndOfFrame();

		var instructions = template.GetInstructionsFor(agent);
		_ = instructions();

		yield return new WaitForEndOfFrame();

		Assert.True(agent.activeSelf);
	}

	[UnityTest]
	public IEnumerator EnableAfterCooldown() {
		var agent = new GameObject();
		var runner = new GameObject().AddComponent<MockMB>();
		var template = new DisableDuringCooldownTemplate { cooldown = 0.3f };

		yield return new WaitForEndOfFrame();

		var instructions = template.GetInstructionsFor(agent);

		runner.StartCoroutine(instructions()!.GetEnumerator());

		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.3f);

		Assert.True(agent.activeSelf);
	}
}
