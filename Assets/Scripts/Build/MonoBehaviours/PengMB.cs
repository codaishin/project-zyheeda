using UnityEngine;

public interface IApplicable<T>
{
	void Apply(T value);
	void Release(T value);
}

public class PengMB : MonoBehaviour, IApplicable<Transform>
{
	public void Apply(Transform value) {
		Debug.Log($"PENG on {value}");
	}

	public void Release(Transform value) {

	}
}
