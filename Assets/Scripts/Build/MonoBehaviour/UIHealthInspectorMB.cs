using System;
using UnityEngine.UI;

public class UIHealthInspectorMB : BaseUIInspectorMB<Health>
{
	public Text? text;
	public Image? image;

	public override void Set(Health value) {
		if (this.text == null || this.image == null) throw this.NullError();

		IFormatProvider usFmt = new System.Globalization.CultureInfo("en-US");
		string hp = value.hp.ToString("F1", usFmt);
		string maxHp = value.maxHp.ToString("F1", usFmt);
		this.text.text = $"{hp}/{maxHp}";
		this.image.fillAmount = value.hp / value.maxHp;
	}
}
