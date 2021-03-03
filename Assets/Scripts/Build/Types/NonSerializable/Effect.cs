using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
	public float duration;

	public event Action OnApply;
	public event Action<float> OnMaintain;
	public event Action OnRevert;

	public void Apply()
	{
		this.OnApply?.Invoke();
	}

	public void Maintain(float delta)
	{
		this.duration -= delta;
		this.OnMaintain?.Invoke(delta);
	}

	public void Revert()
	{
		this.OnRevert?.Invoke();
	}
}
