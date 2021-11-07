using System.Linq;

public class MapEquipmentToItemContentMB :
	BaseMapValueToContentMB<Equipment, ItemMB[], ItemContentMB>
{
	public override ItemMB[] MapValueToContent(Equipment source) {
		return source.Select(r => r.value ?? throw r.NullError()).ToArray();
	}
}
