public interface IHasSkills<TSkill>
	where TSkill :
		IHasBegin
{
	TSkill[] Skills { get; }
}
