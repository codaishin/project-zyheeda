using System;
using UnityEngine;

public abstract class BaseStackFactorySO : ScriptableObject, IStackFactory
{
	public abstract IStack Create(
		Func<Effect, Finalizable> effectToRoutine,
		Action<Finalizable> onPull = default,
		Action<Finalizable> onCancel = default
	);
}
