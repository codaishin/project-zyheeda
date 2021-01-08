using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectilePathing
{
	IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target);
}
