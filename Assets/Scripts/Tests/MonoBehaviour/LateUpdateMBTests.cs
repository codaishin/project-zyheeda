using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LateUpdateMBTests : TestCollection
{
	private class UpdateReaderMB : MonoBehaviour
	{
		public enum Time { None = default, Early, Late }

		private bool called;
		public Time time;

		public void Call() => this.called = true;

		private void Update()
		{
			if (this.called && this.time == Time.None) this.time = Time.Early;
		}

		private void LateUpdate()
		{
			if (this.called && this.time == Time.None) this.time =  Time.Late;
		}
	}

	[UnityTest]
	public IEnumerator OnLateUpdateNotNullAfterStart()
	{
		var updateMB = new GameObject("obj").AddComponent<LateUpdateMB>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(updateMB.onLateUpdate);
	}

	[UnityTest]
	public IEnumerator OnLateUpdateNullDefault()
	{
		var updateMB = new GameObject("obj").AddComponent<LateUpdateMB>();

		Assert.Null(updateMB.onLateUpdate);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnLateUpdateCalled()
	{
		var called = 0;
		var updateMB = new GameObject("obj").AddComponent<LateUpdateMB>();

		yield return new WaitForEndOfFrame();

		updateMB.onLateUpdate.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnLateUpdateCalledLate()
	{
		var updateMB = new GameObject("obj").AddComponent<LateUpdateMB>();
		var updateReaderMB = updateMB.gameObject.AddComponent<UpdateReaderMB>();

		yield return new WaitForEndOfFrame();

		updateMB.onLateUpdate.AddListener(() => updateReaderMB.Call());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(UpdateReaderMB.Time.Late, updateReaderMB.time);
	}
}
