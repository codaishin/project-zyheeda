using UnityEngine;

public static class ReferenceExtensions
{
	public static C Get<C>(this Reference reference, ref C buffer)
		where C: Component
	{
		if (!buffer) {
			buffer = reference.GameObject.GetComponent<C>();
		}
		return buffer;
	}
}
