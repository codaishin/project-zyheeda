using UnityEngine;

public interface ISheet : IGetGameObject
{
	Attributes Attributes { get; }

	int Hp { get; set; }
}
