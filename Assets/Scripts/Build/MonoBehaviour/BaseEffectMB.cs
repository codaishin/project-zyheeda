using UnityEngine;

[RequireComponent(typeof(BaseItemMB))]
public abstract class BaseEffectMB : MonoBehaviour
{
	public BaseItemMB Item { get; private set; }

	public abstract void Apply(in SkillMB skill, in GameObject target);

	private void Awake()
	{
		this.Item = this.GetComponent<BaseItemMB>();
	}

	private void OnEnable()
	{
		this.Item.Effects.Add(this.GetInstanceID(), this);
	}

	private void OnDisable()
	{
		this.Item.Effects.Remove(this.GetInstanceID());
	}
}
