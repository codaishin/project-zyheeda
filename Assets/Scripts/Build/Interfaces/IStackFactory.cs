using System;

public interface IStackFactory
{
	IStack Create(
		Func<Effect, Finalizable> effectToRoutine,
		Action<Finalizable> onPull = default,
		Action<Finalizable> onCancel = default
	);
}
