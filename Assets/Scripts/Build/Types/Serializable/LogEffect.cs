using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffect, ISetGameObject, IGetGameObject
{
	public GameObject gameObject { get; set; }

	public void Apply(in GameObject target, in Attributes attributes)
	{
		Debug.Log($"Applied to {target} with: {attributes}");
	}
}
