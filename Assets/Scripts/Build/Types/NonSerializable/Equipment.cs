using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : RecordsArray<EquipmentSlot, ItemMB>
{
	public Equipment(
		Func<Record<EquipmentSlot, ItemMB>[]> get,
		Action<Record<EquipmentSlot, ItemMB>[]> set
	) : base(get, set) {}
}
