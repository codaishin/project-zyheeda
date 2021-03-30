using System;
using System.Collections.Generic;

public interface IStack<TEffectRoutineFactory>
	where TEffectRoutineFactory : IEffectRoutineFactory
{
	TEffectRoutineFactory Factory { get; set; }
	IEnumerable<Effect> Effects  {get; }
	event Action<Finalizable> OnPush;
	event Action<Finalizable> OnCancel;
	void Push(Effect effect);
	void Cancel();
}
