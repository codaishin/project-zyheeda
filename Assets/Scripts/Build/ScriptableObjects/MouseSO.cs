using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSO : BaseMouseSO
{
	public override Vector3 Position => Input.mousePosition;
}
