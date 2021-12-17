using UnityEngine;

public class Approach : BaseApproach<Vector3>
{
	public override Vector3 GetPosition(in Vector3 target) => target;

	public override float GetTimeDelta() => Time.fixedDeltaTime;

	public override void OnPositionUpdated(
		in Transform current,
		in Vector3 target
	) {
		current.LookAt(target);
	}
}

public class ApproachMB : BaseApproachMB<Approach, Vector3> { }
