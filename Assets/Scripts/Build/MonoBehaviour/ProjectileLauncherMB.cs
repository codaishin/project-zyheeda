using UnityEngine;

[RequireComponent(typeof(MagazineMB))]
public class ProjectileLauncherMB : MonoBehaviour
{
	public Transform spawnProjectilesAt;

	public MagazineMB Magazine { get; private set; }

	private void Awake()
	{
		this.Magazine = this.GetComponent<MagazineMB>();
	}
}
