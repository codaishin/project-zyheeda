using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Mouse")]
public class MouseSO : BaseMouseSO
{
	public override Vector3 Position => Input.mousePosition;
}
