using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WalkMB : MonoBehaviour
{
	public bool walk;

	private Animator? animator;

	private void Start() {
		this.animator = this.GetComponent<Animator>();
	}

	private void Update() {
		this.animator!.SetBool("walk", this.walk);
	}
}
