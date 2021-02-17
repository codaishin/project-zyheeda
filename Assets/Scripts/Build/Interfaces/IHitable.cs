using UnityEngine;

public interface IHitable
{
	bool TryHit(in Attributes attributes);
	GameObject gameObject { get; }
}
