using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMB : MonoBehaviour
{
	private GameObject agentObject;

	public Reference agent;
	public BaseSkillBehaviourSO behaviour;

	private void Start() =>
		this.agentObject = this.agent.GameObject;

	public void Apply(GameObject target) =>
		this.behaviour.Apply(this.agentObject, target);
}
