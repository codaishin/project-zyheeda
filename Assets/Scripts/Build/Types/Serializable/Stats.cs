using System;

[Serializable]
public struct Stats
{
	public int body;
	public int mind;
	public int spirit;

	public static Stats operator+(in Stats a, in Stats b) => new Stats {
		body = a.body + b.body,
		mind = a.mind + b.mind,
		spirit = a.spirit + b.spirit,
	};
}
