using System;
using UnityEngine;

public interface IItem
{
	Action? GetUseOn(Transform target);
	Animation.State ActiveState { get; }
	float UseAfterSeconds { get; }
	float LeaveActiveStateAfterSeconds { get; }
}

public interface ITransform
{
	Transform transform { get; }
}

public interface IIdleAnimation
{
	Animation.Stance IdleStance { get; }
}

public interface IComplexItem : IItem, IIdleAnimation, ITransform { }

public class ItemMB : MonoBehaviour, IComplexItem
{
	public Animation.Stance idleStance;
	public Animation.State activeState;
	public float useAfterSeconds;
	public float resetAfterSeconds;

	public Animation.Stance IdleStance =>
		this.idleStance;
	public Animation.State ActiveState =>
		this.activeState;
	public float UseAfterSeconds =>
		this.useAfterSeconds;
	public float LeaveActiveStateAfterSeconds =>
		this.resetAfterSeconds;

	public Action? GetUseOn(Transform target) {
		return () => Debug.Log($"Use on {target}");
	}
}
