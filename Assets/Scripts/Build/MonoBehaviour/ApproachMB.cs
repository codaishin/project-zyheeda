using UnityEngine;

public class Approach : BaseApproach<Vector3>
{
	public override Vector3 GetPosition(in Vector3 target) => target;

	public override float GetTimeDelta() => Time.fixedDeltaTime;

	public override void PostUpdate(in Transform transform, in Vector3 target) {}
}

public class ApproachMB : BaseApproachMB<Approach> { }
