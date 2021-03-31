using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStack
{
	IEnumerable<Effect> Effects { get; }
	void Push(Effect effect);
	void Cancel();
}
