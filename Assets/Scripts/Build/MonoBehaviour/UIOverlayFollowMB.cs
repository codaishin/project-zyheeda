using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIOverlayFollowMB : MonoBehaviour
{
	private Camera ctrlCam;
	private RectTransform rect;
	private Vector2 origDelta;

	public Reference cam;
	public float correctionFactor = 1f;

	public Collider Target { get; set; }

	private void Start()
	{
		this.ctrlCam = this.cam.GameObject.RequireComponent<Camera>();
		this.rect = this.GetComponent<RectTransform>();
		this.origDelta = this.rect.sizeDelta;
	}

	private float GetCamTargetDistance()
	{
		Vector3 camTargetDirection = this.Target.transform.position - this.ctrlCam.transform.position;
		return Vector3.Project(camTargetDirection, this.ctrlCam.transform.forward).magnitude;
	}

	private float GetTargetMaxBoundSize() => this.Target.bounds.size.AsEnumerable().Max();

	private void LateUpdate()
	{
		float dist = this.GetCamTargetDistance();
		float maxBoundSize = this.GetTargetMaxBoundSize();

		this.transform.position = this.ctrlCam.WorldToScreenPoint(this.Target.transform.position);
		this.rect.sizeDelta = new Vector2(
			this.correctionFactor * maxBoundSize * this.origDelta.x / dist,
			this.correctionFactor * maxBoundSize * this.origDelta.y / dist
		);
	}
}
