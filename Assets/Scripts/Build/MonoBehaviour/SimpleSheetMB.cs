using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSheetMB : MonoBehaviour, IAttributes
{
	public Attributes attributes;

	public Attributes Attributes => this.attributes;
}
