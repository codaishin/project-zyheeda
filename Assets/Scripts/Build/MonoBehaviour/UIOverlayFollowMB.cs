using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIOverlayFollowMB : MonoBehaviour
{
	private Camera ctrlCam;
	private RectTransform rect;
	private Vector2 origDelta;

	public Reference cam;
	public Collider target;
	public float correctionFactor = 1f;

	private void Start()
	{
		this.ctrlCam = this.cam.GameObject.RequireComponent<Camera>();
		this.rect = this.GetComponent<RectTransform>();
		this.origDelta = this.rect.sizeDelta;
	}

	private void LateUpdate()
	{
		this.transform.position = this.ctrlCam.WorldToScreenPoint(this.target.transform.position);
		Vector3 dir = this.target.transform.position - this.ctrlCam.transform.position;
		Vector3 norm = this.ctrlCam.transform.forward;
		Vector3 planeDist = Vector3.Project(dir, norm);
		float dist = planeDist.magnitude;
		float maxBoundSize = this.target.bounds.size.AsEnumerable().Max();

		this.rect.sizeDelta = new Vector2(
			this.correctionFactor * maxBoundSize * this.origDelta.x / dist,
			this.correctionFactor * maxBoundSize * this.origDelta.y / dist
		);
	}
}
