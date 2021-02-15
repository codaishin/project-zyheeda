using UnityEngine;

public static class ProjectileMBExtensions
{
	public static TProjectile AddComponent<TProjectile>(this GameObject gameObject, in MagazineMB magazine)
		where TProjectile: ProjectileMB => ProjectileMB.Init<TProjectile>(gameObject, magazine);
}

public class ProjectileMB : MonoBehaviour
{
	public MagazineMB Magazine { get; private set; }

	public static
	TProjectile Init<TProjectile>(in GameObject gameObject, in MagazineMB magazine)
		where TProjectile: ProjectileMB
	{
		TProjectile projectile = gameObject.AddComponent<TProjectile>();
		projectile.Magazine = magazine;
		return projectile;
	}

	public void Store()
	{
		this.transform.SetParent(this.Magazine.transform);
		this.gameObject.SetActive(false);
	}
}
