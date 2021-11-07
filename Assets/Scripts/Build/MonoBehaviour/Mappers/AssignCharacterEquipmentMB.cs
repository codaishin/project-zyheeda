using System;

public class AssignCharacterEquipmentMB :
	BaseMapFieldToContentMB<Reference, Equipment>
{
	private static Equipment GetEquipmentFrom(CharacterSheetMB sheet) {
		Equipment? equipment = default;
		Action getOrThrow = sheet.UseSection((ref Equipment s) => equipment = s);
		getOrThrow();
		return equipment!;
	}

	public override Equipment MapFieldToContentValue(Reference fieldValue) {
		CharacterSheetMB sheet = this.value.GameObject
			.GetComponentInChildren<CharacterSheetMB>();
		return AssignCharacterEquipmentMB.GetEquipmentFrom(sheet);
	}
}
