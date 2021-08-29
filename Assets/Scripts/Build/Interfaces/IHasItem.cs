public interface IHasItem<TSkill>
	where TSkill :
		IHasBegin
{
	IHasSkills<TSkill> Item { get; set; }
}
