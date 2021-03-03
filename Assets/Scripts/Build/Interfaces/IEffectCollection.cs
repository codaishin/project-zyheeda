using System;
using UnityEngine;

public interface IEffectCollection<TSheet>
	where TSheet: ISheet
{
	bool GetHandle(GameObject target, out Action<TSheet> handle);
}
