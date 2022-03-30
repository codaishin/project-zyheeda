using UnityEngine;

public class PengMB : MonoBehaviour, IApplicable<Transform>
{
	public void Apply(Transform value) {
		Debug.Log($"PENG on {value}");
	}

	public void Release(Transform value) {

	}
}
