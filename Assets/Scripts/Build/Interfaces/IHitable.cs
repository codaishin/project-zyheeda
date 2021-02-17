using UnityEngine;

public interface IHitable : IGetGameObject
{
	bool TryHit(in Attributes attributes);
}
