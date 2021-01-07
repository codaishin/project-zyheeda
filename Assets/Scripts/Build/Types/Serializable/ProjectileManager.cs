using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileManager : IProjectileManager
{
	public IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target)
	{
		yield break;
	}
}
