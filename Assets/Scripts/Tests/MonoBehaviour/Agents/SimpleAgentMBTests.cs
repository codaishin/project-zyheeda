using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleAgentMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator GameObjectIsAgent() {
		var agent = new GameObject().AddComponent<SimpleAgentMB>();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent.gameObject, agent.Agent);
	}
}
