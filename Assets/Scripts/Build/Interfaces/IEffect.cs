using UnityEngine;

public interface IEffect
{
	void Apply(in GameObject target, in Attributes attributes);
}
