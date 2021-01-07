using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileManager
{
	IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target);
}
