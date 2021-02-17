using System.Collections.Generic;
using UnityEngine;

public interface ICast
{
	IEnumerator<WaitForFixedUpdate> Apply(GameObject target);
}
