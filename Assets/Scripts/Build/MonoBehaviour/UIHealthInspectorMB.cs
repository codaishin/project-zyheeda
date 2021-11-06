using UnityEngine.UI;

public class UIHealthInspectorMB : BaseUIInspectorMB<Health>
{
	public Text? text;
	public Image? image;

	public override void Set(Health value) {
		if (this.text == null || this.image == null) throw this.NullError();
		this.text.text = $"{value.hp:F1}/{value.maxHp:F1}";
		this.image.fillAmount = value.hp / value.maxHp;
	}
}
