using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public static class Tools
{
	public static void AssertEqual(in Vector3 expected, in Vector3 actual)
	{
		if (expected != actual) {
			throw new AssertionException(
				$"Expected: ({expected.x}, {expected.y}, {expected.z})\n" +
				$" But was: ({actual.x}, {actual.y}, {actual.z})"
			);
		}
	}

	public static void AssertEqual(in Vector3 expected, in Vector3 actual, in Vector3 delta)
	{
		if (Mathf.Abs(expected.x - actual.x) > delta.x ||
		    Mathf.Abs(expected.y - actual.y) > delta.y ||
		    Mathf.Abs(expected.z - actual.z) > delta.z) {
			throw new AssertionException(
				$"Expected: ({expected.x} +/- {delta.x}, {expected.y} +/- {delta.y}, {expected.z} +/- {delta.z})\n" +
				$" But was: ({actual.x}, {actual.y}, {actual.z})"
			);
		}
	}
}
