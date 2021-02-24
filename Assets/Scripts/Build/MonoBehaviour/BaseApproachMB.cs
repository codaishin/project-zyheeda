using UnityEngine;

public abstract class BaseApproachMB<TAppraoch> : MonoBehaviour
	where TAppraoch: IApproach<Vector3>, new()
{
	private Transform agentTransform;

	public Reference agent;
	public float deltaPerSecond;
	public TAppraoch appraoch = new TAppraoch();

	private void Start() => this.agentTransform = this.agent.GameObject.transform;

	public void Apply(Vector3 position)
	{
		this.StopAllCoroutines();
		this.StartCoroutine(this.appraoch.Apply(this.agentTransform, position, this.deltaPerSecond));
	}
}
