using UnityEngine;

public class GroundSkillMB : MonoBehaviour
{
	public BaseSkillMB skill;
	public GameObject selector;


	public void Begin(Vector3 position)
	{
		this.selector.transform.position = position;
		this.skill.Begin(this.selector);
	}
}
