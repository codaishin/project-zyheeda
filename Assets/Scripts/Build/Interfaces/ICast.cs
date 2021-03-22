using System.Collections.Generic;
using UnityEngine;

public interface ICast<TSheet>
{
	IEnumerator<WaitForFixedUpdate> Apply(TSheet target);
}
