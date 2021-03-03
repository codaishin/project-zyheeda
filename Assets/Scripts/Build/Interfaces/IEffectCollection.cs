using System;
using UnityEngine;

public interface IEffectCollection
{
	bool GetHandle(GameObject target, out Action<Attributes> handle);
}
