using NUnit.Framework;

public class AttributesTests : TestCollection
{
	[Test]
	public void Add() {
		var a = new Attributes { body = 10, mind = 20, spirit = 44 };
		var b = new Attributes { body = 11, mind = -5, spirit = 42 };
		var c = a + b;

		Assert.AreEqual(
			(21, 15, 86),
			(c.body, c.mind, c.spirit)
		);
	}

	[Test]
	public void String() {
		var a = new Attributes { body = 10, mind = 20, spirit = 44 };

		Assert.AreEqual("(body: 10, mind: 20, spirit: 44)", a.ToString());
	}
}
