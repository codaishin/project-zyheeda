using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/KeyInputGroup")]
public class KeyInputGroupSO : ScriptableObject
{
	public BaseKeyInputSO inputSO;
	public RecordArray<EventSO, KeyInputItem> input;

	public void Apply() => this.input.Records
		.Where(this.GotInput)
		.ForEach(KeyInputGroupSO.Raise);

	private
	bool GotInput(Record<EventSO, KeyInputItem> record) => this.inputSO.GetKey(
		record.value.keyCode,
		record.value.keyState
	);

	private static
	void Raise(Record<EventSO, KeyInputItem> record) => record.key.Raise();
}
