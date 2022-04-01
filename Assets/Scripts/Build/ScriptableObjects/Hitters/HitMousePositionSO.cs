using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/MousePosition")]
public class HitMousePositionSO : ScriptableObject, IHit
{
	public ReferenceSO? cameraSO;
	public BaseInputConfigSO? inputConfigSO;
	public LayerMask constraint;


	[NonSerialized] private Camera? camera;
	[NonSerialized] private InputAction? mousePosition;

	private Camera Camera =>
		this.camera ??
		(this.camera = this.cameraSO!.GameObject.RequireComponent<Camera>());

	private InputAction MousePosition =>
		this.mousePosition ??
		(this.mousePosition = this.inputConfigSO![InputEnum.Action.MousePosition]);

	private bool Try(out RaycastHit hit) {
		Vector2 mousePosition = this.MousePosition.ReadValue<Vector2>();
		Ray ray = this.Camera.ScreenPointToRay(mousePosition);
		Vector3 origin = ray.origin;
		Vector3 direction = ray.direction;
		float infinity = float.MaxValue;

		return this.constraint == default
			? Physics.Raycast(origin, direction, out hit, infinity)
			: Physics.Raycast(origin, direction, out hit, infinity, this.constraint);
	}

	public Func<T?> Try<T>(GameObject _) where T : Component => () =>
		this.Try(out RaycastHit hit) && hit.transform.TryGetComponent(out T target)
			? target
			: null;


	public Func<Vector3?> TryPoint(GameObject _) => () =>
		this.Try(out RaycastHit hit)
			? hit.point
			: null;
}
