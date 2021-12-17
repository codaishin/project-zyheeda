using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorMB : MonoBehaviour
{
	public Animator? animator;

	public void Walk(bool value) {
		this.animator!.SetBool("walk", value);
	}
}
