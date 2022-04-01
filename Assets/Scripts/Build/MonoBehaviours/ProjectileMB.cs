using UnityEngine;

public class ProjectileMB : MonoBehaviour, IApplicable<Transform>
{
	public Transform? target;
	public Reference<IApplicable> apply;

	public void Apply(Transform value) {
		this.target = value;
		this.apply.Value!.Apply();
	}

	public void Release(Transform value) { }
}
