using UnityEngine;

public class BaseInstructionsTemplateMB<TTemplate> :
	MonoBehaviour,
	IInstructionsTemplate
	where TTemplate :
		IInstructionsTemplate,
		new()
{
	[SerializeField]
	private TTemplate template = new TTemplate();

	public TTemplate Template => this.template;

	public ExternalInstructionsFn GetInstructionsFor(GameObject agent) {
		return this.template.GetInstructionsFor(agent);
	}
}
