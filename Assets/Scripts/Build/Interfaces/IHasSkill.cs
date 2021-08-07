public interface IHasSkill<TSkill>
	where TSkill :
		IHasBegin
{
	TSkill Skill { get; set; }
}
