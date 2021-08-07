using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseContentMBTests : TestCollection
{
	class MockContentElem : MonoBehaviour, IHasValue<int>
	{
		public int Value { get; set; }
		public int arbitrary;
	}

	class MockContentMB : BaseContentMB<int, MockContentElem> { }

	[UnityTest]
	public IEnumerator AddElement() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elems = new int[] { 1 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elems;

		CollectionAssert.AreEqual(
			elems,
			content
				.GetComponentsInChildren<MockContentElem>(includeInactive: false)
				.Select(c => c.Value)
		);
	}

	[UnityTest]
	public IEnumerator AddElements() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elems = new int[] { 1, 2 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elems;

		CollectionAssert.AreEqual(
			elems,
			content
				.GetComponentsInChildren<MockContentElem>(includeInactive: false)
				.Select(c => c.Value)
		);
	}

	[UnityTest]
	public IEnumerator AddElementsRepeatedly() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elemsA = new int[] { 1, 2 };
		var elemsB = new int[] { 3, 4, 5 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elemsA;
		content.Value = elemsB;

		CollectionAssert.AreEqual(
			elemsB,
			content
				.GetComponentsInChildren<MockContentElem>(includeInactive: false)
				.Select(c => c.Value)
		);
	}

	[UnityTest]
	public IEnumerator NotMoreChildrenThanLargestElemsLength() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elemsA = new int[] { 1, 2, 3 };
		var elemsB = new int[] { 4, 5 };
		var elemsC = new int[] { 6, 7, 8 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elemsA;
		content.Value = elemsB;
		content.Value = elemsC;

		yield return new WaitForFixedUpdate();

		MockContentElem[] contents = content
			.GetComponentsInChildren<MockContentElem>(includeInactive: true);
		Assert.AreEqual(3, contents.Length);
	}


	[UnityTest]
	public IEnumerator AddThreeThanTwoThanThree() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elemsA = new int[] { 1, 2, 3 };
		var elemsB = new int[] { 4, 5 };
		var elemsC = new int[] { 6, 7, 8 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elemsA;
		content.Value = elemsB;
		content.Value = elemsC;

		CollectionAssert.AreEqual(
			elemsC,
			content
				.GetComponentsInChildren<MockContentElem>(includeInactive: false)
				.Select(c => c.Value)
		);
	}

	[UnityTest]
	public IEnumerator UsePrefab() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elems = new int[] { 1, 2, 3 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();
		content.prefab.arbitrary = 44;

		yield return new WaitForFixedUpdate();

		content.Value = elems;

		CollectionAssert.AreEqual(
			new int[] { 44, 44, 44 },
			content
				.GetComponentsInChildren<MockContentElem>(includeInactive: false)
				.Select(c => c.arbitrary)
		);
	}

	[UnityTest]
	public IEnumerator ReturnValue() {
		var content = new GameObject("content").AddComponent<MockContentMB>();
		var elems = new int[] { 1, 2, 3 };
		content.prefab = new GameObject("prefav").AddComponent<MockContentElem>();

		yield return new WaitForFixedUpdate();

		content.Value = elems;

		Assert.AreSame(elems, content.Value);
	}
}
