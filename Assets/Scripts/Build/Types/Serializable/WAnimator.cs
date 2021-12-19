using System;
using UnityEngine;

[Serializable]
public class WAnimator : IAnimator
{
	public Animator? animator;

	public void SetBool(string name, bool value) {
		this.animator!.SetBool(name, value);
	}
}