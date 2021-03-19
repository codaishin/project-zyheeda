using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
	public static IEnumerable<float> AsEnumerable(this Vector3 vector)
	{
		yield return vector.x;
		yield return vector.y;
		yield return vector.z;
	}
}
