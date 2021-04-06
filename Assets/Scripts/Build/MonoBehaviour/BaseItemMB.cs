using UnityEngine;

public abstract class BaseItemMB<TSheet> : MonoBehaviour
{
	private ISkill<TSheet>[] skills;
	private TSheet sheet;

	public TSheet Sheet {
		get => this.sheet;
		set {
			this.sheet = value;
			this.AssignSkillsSheet();
		}
	}

	private void Start()
	{
		this.skills = this.GetComponents<ISkill<TSheet>>();
		this.AssignSkillsSheet();
	}

	private void AssignSkillsSheet()
	{
		this.skills?.ForEach(s => s.Sheet = this.sheet);
	}
}
