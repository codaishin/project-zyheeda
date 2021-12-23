using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEventMapperMBTests : TestCollection
{
	private class MockRayCastHit : MonoBehaviour, IRayCastHit
	{
		public RaycastHit? hit;

		public Maybe<RaycastHit> TryHit() => this.hit == null
			? Maybe.None<RaycastHit>()
			: Maybe.Some(this.hit.Value);
	}

	private class MockMapperMB : BaseRayCastHitMapperMB<MockRayCastHit, RaycastHit>
	{
		public override Maybe<RaycastHit> Map(Maybe<RaycastHit> hit) => hit;
	}

	[UnityTest]
	public IEnumerator ApplyNone() {
		RaycastHit? called = new RaycastHit();
		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();
		var hitter = mapper.gameObject.AddComponent<MockRayCastHit>();

		yield return new WaitForEndOfFrame();

		hitter.hit = null;
		mapper.onValueMapped.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		mapper.Apply();

		Assert.Null(called);
	}

	[UnityTest]
	public IEnumerator ApplySome() {
		RaycastHit? called = null;
		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();
		var hitter = mapper.gameObject.AddComponent<MockRayCastHit>();

		yield return new WaitForEndOfFrame();

		hitter.hit = new RaycastHit();
		mapper.onValueMapped.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		mapper.Apply();

		Assert.AreEqual(hitter.hit, called);
	}
}
