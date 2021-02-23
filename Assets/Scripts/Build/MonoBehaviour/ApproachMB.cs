using UnityEngine;

public class ApproachMB : BaseApproachMB
{
	protected override
	Movement.ApproachFunc<Vector3> Approach { get; } = Movement.GetApproach(
		(Vector3 position) => position,
		() => Time.fixedDeltaTime
	);
}
