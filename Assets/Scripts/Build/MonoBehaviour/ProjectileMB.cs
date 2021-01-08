using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMB : MonoBehaviour
{
	public MagazineMB Magazine { get; set; }

	private void OnDisable()
	{
		if (this.Magazine) {
			this.transform.parent = Magazine.transform;
		}
	}
}
