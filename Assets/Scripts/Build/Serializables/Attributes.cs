using System;

[Serializable]
public struct Attributes
{
	public int body;
	public int mind;
	public int spirit;

	public static Attributes operator+(in Attributes a, in Attributes b) => new Attributes {
		body = a.body + b.body,
		mind = a.mind + b.mind,
		spirit = a.spirit + b.spirit,
	};

	public override string ToString() => $"(body: {this.body}, mind: {this.mind}, spirit: {this.spirit})";
}
