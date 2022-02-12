using System;
using UnityEngine;

public abstract class BaseChannelSO : ScriptableObject
{
	public abstract void AddListener(Action action);
	public abstract void RemoveListener(Action action);
}
