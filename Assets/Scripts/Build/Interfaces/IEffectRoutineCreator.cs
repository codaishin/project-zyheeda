using System;
using System.Collections.Generic;

public interface IEffectRoutineCreator
{
	Finalizable Create(Effect effect, out Action revert);
}
