using UnityEngine;

public abstract class BaseHitSO : ScriptableObject
{
	public abstract IHit Hit {get;}
}

public abstract class BaseHitSO<THit> : BaseHitSO
	where THit : IHit, new()
{
	public THit hit = new THit();

	public override IHit Hit => this.hit;
}
