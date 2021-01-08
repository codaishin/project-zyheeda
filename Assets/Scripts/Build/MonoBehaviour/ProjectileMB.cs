using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMB : MonoBehaviour
{
	public BaseMagazineMB Magazine { get; set; }

	public void Store()
	{
		this.transform.SetParent(this.Magazine.transform);
		this.gameObject.SetActive(false);
	}
}
