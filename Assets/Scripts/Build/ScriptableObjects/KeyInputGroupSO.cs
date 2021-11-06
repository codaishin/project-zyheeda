using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/KeyInputGroup")]
public class KeyInputGroupSO : ScriptableObject
{
	public BaseKeyInputSO? inputSO;
	public RecordArray<EventSO, KeyInputItem>? input;

	public void Apply() => this.input
		.GroupBy(r => r.key)
		.Select(g => g.First())
		.Where(this.GotInput)
		.ForEach(KeyInputGroupSO.Raise);

	private bool GotInput(Record<EventSO, KeyInputItem> record) {
		if (this.inputSO == null) throw this.NullError();
		return this.inputSO.GetKey(
			record.value.keyCode,
			record.value.keyState
		);
	}

	private static void Raise(Record<EventSO, KeyInputItem> record) {
		record.key.Raise();
	}

	public void OnValidate() {
		if (this.input != null) {
			this.input.SetNamesFromKeys(duplicateLabel: "__duplicate__");
		}
	}
}
