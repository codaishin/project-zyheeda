using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetMB : MonoBehaviour, IAttributes, IHp
{
	public Attributes attributes;
	public int hp;

	public Attributes Attributes => this.attributes;
	public int Hp {
		get => this.hp;
		set => this.hp = value;
	}
}
