using System;

public delegate IStack GetStackFunc(
	Func<Effect, Finalizable> effectToRoutine,
	Action<Finalizable>? onPull = default,
	Action<Finalizable>? onCancel = default
);
