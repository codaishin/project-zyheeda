using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSkillMB : MonoBehaviour
{
	private GameObject selectorObject;

	public SkillMB skill;
	public Reference selector;

	private void Start() => this.selectorObject = this.selector.GameObject;

	public void Apply(Vector3 position)
	{
		this.selectorObject.transform.position = position;
		this.skill.Apply(this.selectorObject);
	}
}
