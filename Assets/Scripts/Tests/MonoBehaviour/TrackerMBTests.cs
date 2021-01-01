using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class TrackerMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator TrackPosition()
	{
		var trackerMB = new GameObject("tracker").AddComponent<TrackerMB>();
		var agent = new GameObject("agent");
		var target = new GameObject("target");

		trackerMB.agent = agent;
		trackerMB.target = target;

		yield return new WaitForEndOfFrame();

		target.transform.position = Vector3.right;
		trackerMB.Track();

		Tools.AssertEqual(Vector3.right, agent.transform.position);
	}
}
