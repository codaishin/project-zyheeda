using UnityEngine;

public abstract class BaseInstructionsTemplateSO<TTemplate> :
	ScriptableObject,
	IInstructionsTemplate
	where TTemplate :
		IInstructionsTemplate,
		new()
{
	[SerializeField]
	private TTemplate template = new TTemplate();

	public TTemplate Template => this.template;

	public InstructionsFunc GetInstructionsFor(GameObject agent) {
		return this.template.GetInstructionsFor(agent);
	}
}
