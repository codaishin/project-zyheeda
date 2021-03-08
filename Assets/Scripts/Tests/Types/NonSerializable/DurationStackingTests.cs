using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DurationStackingTests
{
	[Test]
	public void AddNew()
	{
		var routines = new List<Finalizable>();
		var routine = new Finalizable();
		var stacking = new DurationStacking();

		stacking.Add(routine, routines, _ => { });

		CollectionAssert.AreEqual(
			new Finalizable[] { routine },
			routines.Select(f => f.wrapped)
		);
	}

	[Test]
	public void AddNewOnAdd()
	{
		var called = default(Finalizable);
		var routines = new List<Finalizable>();
		var routine = new Finalizable();
		var stacking = new DurationStacking();

		stacking.Add(routine, routines, a => called = a);

		Assert.AreSame(routines[0], called);
	}

	[Test]
	public void DontAddSecond()
	{
		var routines = new List<Finalizable>();
		var routine = new Finalizable();
		var stacking = new DurationStacking();

		stacking.Add(routine, routines, _ => { });
		stacking.Add(routine, routines, _ => { });

		Assert.AreEqual(1, routines.Count);
	}

	[Test]
	public void ConcatSecond()
	{
		var called = string.Empty;
		IEnumerator get(string v) {
			called += v;
			yield return null;
		}
		var routines = new List<Finalizable>();
		var routineA = new Finalizable{ wrapped = get("f") };
		var routineB = new Finalizable{ wrapped = get("s") };
		var stacking = new DurationStacking();

		stacking.Add(routineA, routines, _ => { });
		stacking.Add(routineB, routines, _ => { });

		var concat = routines[0];
		concat.MoveNext();
		concat.MoveNext();

		Assert.AreEqual("fs", called);
	}
}
