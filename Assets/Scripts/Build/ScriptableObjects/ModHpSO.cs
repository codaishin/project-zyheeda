using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EffectFactories/ModHp")]
public class ModHpSO : BaseModHpSO<CharacterSheetMB, AsDictWrapper<EffectTag, float>> {}
