using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MagazineMB : MonoBehaviour
{
	public GameObject prefab;

	private Transform MakeProjectile()
	{
		return GameObject.Instantiate(this.prefab).transform;
	}

	public Transform GetOrMakeProjectile()
	{
		return this.MakeProjectile();
	}
}
