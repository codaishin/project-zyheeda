using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class UIHealthInspectorMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator SetText()
	{
		var inspector = new GameObject("inspector").AddComponent<UIHealthInspectorMB>();
		var text = new GameObject("text").AddComponent<Text>();
		var image = new GameObject("image").AddComponent<Image>();
		inspector.text = text;
		inspector.image = image;

		yield return new WaitForFixedUpdate();

		inspector.Set(new Health{ hp = 10f, maxHp = 100f });

		Assert.AreEqual("10/100", text.text);
	}

	[UnityTest]
	public IEnumerator SetImageFill()
	{
		var inspector = new GameObject("inspector").AddComponent<UIHealthInspectorMB>();
		var text = new GameObject("text").AddComponent<Text>();
		var image = new GameObject("image").AddComponent<Image>();
		inspector.text = text;
		inspector.image = image;

		yield return new WaitForFixedUpdate();

		inspector.Set(new Health{ hp = 5f, maxHp = 25f });

		Assert.AreEqual(5f / 25f, image.fillAmount);
	}
}
