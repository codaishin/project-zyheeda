using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
	Attributes Attributes { get; }
	GameObject gameObject { get; }
}
