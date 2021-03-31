using System.Collections.Generic;

public interface IStack
{
	IEnumerable<Effect> Effects { get; }
	void Push(Effect effect);
	void Cancel();
}
