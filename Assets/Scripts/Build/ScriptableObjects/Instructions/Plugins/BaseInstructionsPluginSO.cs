using System;
using UnityEngine;

public abstract class BaseInstructionsPluginSO : ScriptableObject
{
	public abstract Action GetOnBegin(GameObject agent);
	public abstract Action GetOnEnd(GameObject agent);
}
