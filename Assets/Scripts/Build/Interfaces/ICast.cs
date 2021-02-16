using System.Collections.Generic;
using UnityEngine;

public interface ICast
{
	IEnumerator<WaitForFixedUpdate> Apply(GameObject target);
	bool Valid(in GameObject target, out IHitable hitable);
}
