using UnityEngine;

public abstract class BaseItemMB<TSkill, TSheet> :
	MonoBehaviour,
	IHasSkills<TSkill>
	where TSkill :
		IHasBegin,
		IHasSheet<TSheet>
{
	private TSkill[] skills;
	private TSheet sheet;

	public TSheet Sheet {
		get => this.sheet;
		set {
			this.sheet = value;
			this.AssignSkillsSheet();
		}
	}

	public TSkill[] Skills => this.skills;

	private void Start() {
		this.skills = this.GetComponents<TSkill>();
		this.AssignSkillsSheet();
	}

	private void AssignSkillsSheet() {
		this.skills?.ForEach(s => s.Sheet = this.sheet);
	}
}
