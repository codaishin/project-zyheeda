using UnityEngine;

public abstract class BaseResistanceMB<TResistance> :
	MonoBehaviour
	where TResistance :
		IRecordArray<EffectTag, float>, new()
{
	public TResistance? resistance;

	private void Start() {
		if (this.resistance == null) {
			this.resistance = new TResistance();
		}
	}

	public void OnValidate() {
		if (this.resistance != null) {
			this.resistance.SetNamesFromKeys(duplicateLabel: "__duplicate__");
		}
	}
}
