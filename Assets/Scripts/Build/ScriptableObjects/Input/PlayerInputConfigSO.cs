using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerInputConfig")]
public class PlayerInputConfigSO : ScriptableObject
{
	private PlayerInputConfig? config;

	public PlayerInputConfig Config {
		get {
			if (this.config == null) {
				this.config = new PlayerInputConfig();
			}
			return this.config;
		}
	}
}
