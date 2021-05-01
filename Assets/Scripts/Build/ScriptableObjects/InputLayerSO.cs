using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/InputLayer")]
public class InputLayerSO : BaseConditionalSO<InputLayer>
{
	public InputLayer CurrentInputLayer { get; set; }

	public override bool Check(InputLayer value) => value == this.CurrentInputLayer;
}
