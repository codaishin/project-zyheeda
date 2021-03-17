using System;

public interface IEffectRoutineFactory
{
	Finalizable Create(Effect effect, out Action revert);
}
