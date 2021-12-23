using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RayCastHitPointMBTests : TestCollection
{
	private class MockRayCastHit : MonoBehaviour, IRayCastHit
	{
		public RaycastHit? hit;

		public Maybe<RaycastHit> TryHit() => this.hit == null
			? Maybe.None<RaycastHit>()
			: Maybe.Some(this.hit.Value);
	}

	private class MockMapperMB : BaseRayCastHitPointMB<MockRayCastHit> { }

	[UnityTest]
	public IEnumerator MapHitNone() {
		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();
		var raycast = mapper.gameObject.AddComponent<MockRayCastHit>();
		raycast.hit = null;

		yield return new WaitForEndOfFrame();

		Assert.IsInstanceOf<None<Vector3>>(mapper.Map());
	}

	[UnityTest]
	public IEnumerator MapHitCollider() {
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		yield return new WaitForEndOfFrame();

		var mapper = new GameObject("obj").AddComponent<MockMapperMB>();
		var raycast = mapper.gameObject.AddComponent<MockRayCastHit>();
		raycast.hit = hit;

		yield return new WaitForEndOfFrame();

		var maybe = mapper.Map();

		maybe.Match(
			some: point => Assert.AreEqual(hit.point, point),
			none: () => Assert.Fail("Map returned None")
		);
	}
}
