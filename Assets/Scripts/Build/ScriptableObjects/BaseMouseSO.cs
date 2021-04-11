using UnityEngine;

public abstract class BaseMouseSO : ScriptableObject, IPosition
{
	public abstract Vector3 Position { get; }
}
