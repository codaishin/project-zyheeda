using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMouseSO : ScriptableObject
{
	public abstract Vector3 Position { get; }
}
