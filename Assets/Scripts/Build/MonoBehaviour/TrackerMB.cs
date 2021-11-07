using UnityEngine;

public class TrackerMB : MonoBehaviour
{
	private Transform? agentTransform;
	private Transform? targetTransform;

	public Reference agent;
	public Reference target;

	private void Start() {
		this.agentTransform = this.agent.GameObject.transform;
		this.targetTransform = this.target.GameObject.transform;
	}

	public void Track() {
		this.agentTransform!.position = this.targetTransform!.position;
	}
}
