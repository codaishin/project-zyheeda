using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class BaseHitableMB : MonoBehaviour
{
	public abstract bool TryHit(in int offense);
}
