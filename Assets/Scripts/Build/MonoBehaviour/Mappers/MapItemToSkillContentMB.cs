using SkillMB = BaseSkillMB<CharacterSheetMB>;

public class MapItemToSkillContentMB :
	BaseMapValueToContentMB<ItemMB, SkillMB[], SkillContentMB>
{
	public override SkillMB[] MapValueToContent(ItemMB item) => item.Skills;
}
