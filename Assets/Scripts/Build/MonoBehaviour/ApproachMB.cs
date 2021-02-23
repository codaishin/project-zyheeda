using UnityEngine;

public class ApproachMB : BaseApproachMB
{
	protected override
	Movement.ApproachFunc<Vector3> Approach { get; } = Movement.GetApproach(
		getPosition: (Vector3 position) => position,
		getTimeDelta: () => Time.fixedDeltaTime
	);
}
