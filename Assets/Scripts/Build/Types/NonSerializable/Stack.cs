using System;
using System.Collections.Generic;

public struct Stack
{
	public Action<Effect> effect;
	public Action cancel;
	public Func<IEnumerable<Effect>> getEffects;
}
