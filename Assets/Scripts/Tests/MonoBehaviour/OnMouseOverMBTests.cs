using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class OnMouseOverMBTests
{
	[UnityTest]
	public IEnumerator NotNull()
	{
		var onMouseOver = new GameObject("obj").AddComponent<OnMouseOverMB>();

		yield return new WaitForEndOfFrame();

		CollectionAssert.AreEqual(
			new object[] { true, true },
			new object[] {
				onMouseOver.onMouseEnter != null,
				onMouseOver.onMouseExit != null
			}
		);
	}

	[Test]
	public void CallbacksNull()
	{
		var onMouseOver = new GameObject("obj").AddComponent<OnMouseOverMB>();
		CollectionAssert.AreEqual(
			new object[] {null, null},
			new object[] { onMouseOver.onMouseEnter, onMouseOver.onMouseExit }
		);
	}

	[UnityTest]
	public IEnumerator OnMouseEnter()
	{
		var called = 0;
		var onMouseOver = new GameObject("obj").AddComponent<OnMouseOverMB>();

		yield return new WaitForEndOfFrame();

		onMouseOver.onMouseEnter.AddListener(() => ++called);

		onMouseOver.Invoke("OnMouseEnter", 0);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator OnMouseExit()
	{
		var called = 0;
		var onMouseOver = new GameObject("obj").AddComponent<OnMouseOverMB>();

		yield return new WaitForEndOfFrame();

		onMouseOver.onMouseExit.AddListener(() => ++called);

		onMouseOver.Invoke("OnMouseExit", 0);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}
}
