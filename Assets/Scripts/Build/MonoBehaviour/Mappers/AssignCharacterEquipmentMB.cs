using System;

public class AssignCharacterEquipmentMB :
	BaseMapFieldToContentMB<Reference, Equipment>
{
	private static Equipment GetEquipmentFrom(CharacterSheetMB sheet) {
		Equipment equipment = default;
		Action getEquipment = sheet.UseSection(
			(ref Equipment section) => equipment = section,
			fallback: null
		);
		getEquipment();
		return equipment;
	}

	public override Equipment MapFieldToContentValue(
		Reference fieldValue
	) {
		CharacterSheetMB sheet = this.value.GameObject
			.GetComponentInChildren<CharacterSheetMB>();
		return AssignCharacterEquipmentMB.GetEquipmentFrom(sheet);
	}
}
