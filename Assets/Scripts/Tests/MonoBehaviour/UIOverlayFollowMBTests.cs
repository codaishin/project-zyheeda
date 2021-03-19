using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UIOverlayFollowMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator FollowTarget()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube);

		ovelray.cam = cam;
		ovelray.target = target.GetComponent<Collider>();
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);

		yield return new WaitForEndOfFrame();

		target.transform.position = Vector3.right;

		yield return new WaitForEndOfFrame();

		var expected = cam.WorldToScreenPoint(Vector3.right);

		Assert.AreEqual(expected, ovelray.transform.position);
	}

	[UnityTest]
	public IEnumerator SizeDeltaScaleByDistance()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube);

		ovelray.cam = cam;
		ovelray.target = target.GetComponent<Collider>();
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);

		var rect = ovelray.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(1, 1);

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(new Vector2(0.1f, 0.1f), rect.sizeDelta);
	}

	[UnityTest]
	public IEnumerator SizeDeltaScaleByDistanceAndCorrectionFactor()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube);

		ovelray.cam = cam;
		ovelray.target = target.GetComponent<Collider>();
		ovelray.correctionFactor = 10f;
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);

		var rect = ovelray.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(1, 1);

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(new Vector2(1f, 1f), rect.sizeDelta);
	}

	[UnityTest]
	public IEnumerator SizeDeltaScaleByDistanceToViewPlane()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube);

		ovelray.cam = cam;
		ovelray.target = target.GetComponent<Collider>();
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);
		target.transform.position = Vector3.left * 100f;

		var rect = ovelray.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(1, 1);

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(new Vector2(0.1f, 0.1f), rect.sizeDelta);
	}

	[UnityTest]
	public IEnumerator SizeDeltaScaleByDistanceBasedOnOriginalSizeDelta()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube);

		ovelray.cam = cam;
		ovelray.target = target.GetComponent<Collider>();
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);

		var rect = ovelray.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(10, 20);

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(new Vector2(1f, 2f), rect.sizeDelta);
	}

	[UnityTest]
	public IEnumerator SizeDeltaScaleByDistanceBasedOnTargetBounds()
	{
		var cam = new GameObject("cam").AddComponent<Camera>();
		var ovelray = new GameObject("obverlay").AddComponent<UIOverlayFollowMB>();
		var target = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();

		ovelray.cam = cam;
		ovelray.target = target;
		cam.transform.position = new Vector3(0, 10, 0);
		cam.transform.LookAt(target.transform);
		target.size = new Vector3(1, 5, 4);

		var rect = ovelray.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(1, 1);

		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(new Vector2(0.5f, 0.5f), rect.sizeDelta);
	}
}
