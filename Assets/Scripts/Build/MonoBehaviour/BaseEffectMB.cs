using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseItemMB))]
public abstract class BaseEffectMB : MonoBehaviour
{
	public BaseItemMB Item { get; private set; }

	private void Awake()
	{
		this.Item = this.GetComponent<BaseItemMB>();
	}
}
