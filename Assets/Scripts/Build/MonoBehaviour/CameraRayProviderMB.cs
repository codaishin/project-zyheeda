using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	public override Ray Ray => default;

	public Camera Camera { get; private set; }

	private void Awake()
	{
		this.Camera = this.GetComponent<Camera>();
	}
}
