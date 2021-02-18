using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSheetMB : MonoBehaviour, ISheet
{
	public Attributes attributes;

	public Attributes Attributes => this.attributes;

	public int Hp { get; set; }
}
