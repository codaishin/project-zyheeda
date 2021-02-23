using UnityEngine;

public abstract class BaseApproachMB : MonoBehaviour
{
	private Transform agentTransform;

	public Reference agent;
	public float deltaPerSecond;

	protected abstract Movement.ApproachFunc<Vector3> Approach { get; }

	private void Start() => this.agentTransform = this.agent.GameObject.transform;

	public void Apply(Vector3 position)
	{
		this.StopAllCoroutines();
		this.StartCoroutine(this.Approach(this.agentTransform, position, this.deltaPerSecond));
	}
}
