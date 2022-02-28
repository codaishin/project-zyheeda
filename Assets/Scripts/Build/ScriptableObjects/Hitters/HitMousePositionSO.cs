using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/MousePosition")]
public class HitMousePositionSO : BaseHitSO
{
	public ReferenceSO? cameraSO;
	public BaseInputConfigSO? inputConfigSO;
	public LayerMask constraint;

	private Camera? camera;
	private InputAction? mousePosition;

	private Camera Camera {
		get {
			if (this.camera == null) {
				this.camera = this.cameraSO!.GameObject.RequireComponent<Camera>();
			}
			return this.camera;
		}
	}

	private InputAction MousePosition {
		get {
			if (this.mousePosition == null) {
				InputEnum.Action action = InputEnum.Action.MousePosition;
				this.mousePosition = this.inputConfigSO![action];
			}
			return this.mousePosition;
		}
	}

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

	public override Maybe<T> Try<T>(T source) {
		T target;
		RaycastHit hit;
		return this.Try(out hit) && hit.transform.TryGetComponent(out target)
			? Maybe.Some(target)
			: Maybe.None<T>();
	}

	public override Maybe<Vector3> TryPoint(Transform source) {
		return this.Try(out RaycastHit hit)
			? Maybe.Some(hit.point)
			: Maybe.None<Vector3>();
	}
}
