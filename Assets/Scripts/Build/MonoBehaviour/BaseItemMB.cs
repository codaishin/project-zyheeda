using UnityEngine;

public abstract class BaseItemMB<TSkill, TSheet> :
	MonoBehaviour,
	IHasSkills<TSkill>
	where TSkill :
		IHasBegin,
		IHasSheet<TSheet>
{
	private TSkill[]? skills;
	private TSheet? sheet;

	public TSheet Sheet {
		get => this.sheet ?? throw this.NullError();
		set => this.MapSheet(value);
	}

	public TSkill[] Skills => this.skills ?? throw this.NullError();

	private void Start() {
		this.skills = this.GetComponents<TSkill>();
		if (this.sheet != null) {
			this.MapSheet(this.sheet);
		}
	}

	private void MapSheet(TSheet sheet) {
		this.sheet = sheet;
		this.skills?.ForEach(s => s.Sheet = sheet);
	}
}
