using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionTarget
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;

	public void Add(Effect effect, EffectTag tag, bool stackDuration)
	{
		if (stackDuration) {
			this.stackDuration.Add(effect, tag);
		} else {
			this.stackIntensity.Add(effect, tag);
		}
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
	}
}
