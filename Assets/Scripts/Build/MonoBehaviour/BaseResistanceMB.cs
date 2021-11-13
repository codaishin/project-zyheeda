using UnityEngine;

public abstract class BaseResistanceMB<TResistance> :
	MonoBehaviour
	where TResistance :
		IRecordArray<EffectTag, float>,
		new()
{
	public TResistance resistance = new TResistance();

	public void OnValidate() {
		this.resistance.SetNamesFromKeys(duplicateLabel: "__duplicate__");
	}
}
