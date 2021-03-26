using System.Linq;
using UnityEngine;

public class ResistanceMB : MonoBehaviour
{
	public Record<EffectTag, float>[] records;

	public Resistance Resistance { get; private set; }

	private void Awake()
	{
		this.Resistance = new Resistance(
			get: () => this.records,
			set: records => this.records = records
		);
	}

	public void OnValidate()
	{
		if (this.records != null) {
			this.records = this.records.Validate().ToArray();
		}
	}
}
