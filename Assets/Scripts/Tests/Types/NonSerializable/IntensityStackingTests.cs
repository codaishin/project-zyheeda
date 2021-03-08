using System.Collections.Generic;
using NUnit.Framework;

public class IntensityStackingTests : TestCollection
{
	[Test]
	public void AddNew()
	{
		var routines = new List<Finalizable>();
		var routine = new Finalizable();
		var stacking = new IntensityStacking();

		stacking.Add(routine, routines, _ => { });

		CollectionAssert.AreEqual(new Finalizable[] { routine }, routines);
	}

	[Test]
	public void AddNewOnAdd()
	{
		var called = default(Finalizable);
		var routines = new List<Finalizable>();
		var routine = new Finalizable();
		var stacking = new IntensityStacking();

		stacking.Add(routine, routines, a => called = a);

		Assert.AreSame(routine, called);
	}
}
