using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionTarget<IntensityManagerMB, DurationManagerMB>
{
	public IntensityManagerMB StackIntensity { get; private set; }
	public DurationManagerMB StackDuration { get; private set; }

	private void Awake()
	{
		this.StackIntensity = this.GetComponent<IntensityManagerMB>();
		this.StackDuration = this.GetComponent<DurationManagerMB>();
	}
}
