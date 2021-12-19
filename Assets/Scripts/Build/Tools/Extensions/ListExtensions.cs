using System.Collections.Generic;

public static class ListExtensions
{
	public static bool RemoveLast<T>(this List<T> list) {
		if (list.Count > 0) {
			list.RemoveAt(list.Count - 1);
			return true;
		}
		return false;
	}
}
