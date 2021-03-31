using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DurationStackFactorySOTests : TestCollection
{
	[Test]
	public void RoutineFromEffect()
	{
		IEnumerator create(Effect effect) {
			effect.Apply();
			yield break;
		}
		var called = false;
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			e => new Finalizable{ wrapped = create(e) },
			onPull: r => r.MoveNext()
		);

		stack.Push(new Effect(apply: () => called = true));

		Assert.True(called);
	}

	[Test]
	public void CancelCallWithPushRoutine()
	{
		IEnumerator create() { yield break; };
		var routines = (onPush: default(Finalizable), onCancel: default(Finalizable));
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routines.onPush = r,
			onCancel: r => routines.onCancel = r
		);

		stack.Push(new Effect());
		stack.Cancel();

		Assert.AreSame(routines.onPush, routines.onCancel);
	}

	[Test]
	public void NoOnCancelWhenNoCancel()
	{
		IEnumerator create() { yield break; };
		var called = false;
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onCancel: _ => called = true
		);

		stack.Push(new Effect());

		Assert.False(called);
	}

	[Test]
	public void EmptyPushDoesNotThrow()
	{
		IEnumerator create() { yield break; };
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(_ => new Finalizable{ wrapped = create() });

		Assert.DoesNotThrow(() => stack.Push(new Effect()));
	}

	[Test]
	public void EmptyCancelDoesNotThrow()
	{
		IEnumerator create() { yield break; };
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(_ => new Finalizable{ wrapped = create() });

		stack.Push(new Effect());
		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void Effects()
	{
		IEnumerator create() { yield break; };
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(_ => new Finalizable{ wrapped = create() });
		var effects = new Effect[] {
			new Effect(),
			new Effect(),
		};

		stack.Push(effects[0]);
		stack.Push(effects[1]);

		CollectionAssert.AreEqual(effects, stack.Effects);
	}

	[Test]
	public void CancelClearsEffects()
	{
		IEnumerator create() { yield break; };
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(_ => new Finalizable{ wrapped = create() });
		var effects = new Effect[] {
			new Effect(),
			new Effect(),
		};

		stack.Push(effects[0]);
		stack.Push(effects[1]);
		stack.Cancel();

		Assert.IsEmpty(stack.Effects);
	}

	[Test]
	public void OnPullOnlyForFirst()
	{
		IEnumerator create() {
			yield return null;
			yield return null;
		};
		var called = 0;
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: _ => ++called
		);

		stack.Push(new Effect());
		var callA = called;
		stack.Push(new Effect());
		var callB = called;

		Assert.AreEqual((1, 1), (callA, callB));
	}

	[Test]
	public void FirstRoutinesAlsoYieldsSubsequentEffects()
	{
		var called = 0;
		IEnumerator create() {
			++called;
			yield return null;
			++called;
			yield return null;
			++called;
		};
		var routine = default(Finalizable);
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routine = r
		);

		stack.Push(new Effect());

		routine.MoveNext();

		stack.Push(new Effect());

		routine.MoveNext();
		routine.MoveNext();
		routine.MoveNext();
		routine.MoveNext();

		Assert.AreEqual(6, called);
	}


	[Test]
	public void FinishedEffectRemovedFromEffects()
	{
		IEnumerator create() {
			yield return null;
			yield return null;
			yield return null;
		}
		var routine = default(Finalizable);
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routine = r
		);

		stack.Push(new Effect());

		routine.MoveNext();
		routine.MoveNext();
		routine.MoveNext();
		routine.MoveNext();

		Assert.IsEmpty(stack.Effects);
	}

	[Test]
	public void YieldsCorrectValues()
	{
		IEnumerator create() {
			yield return 1;
			yield return 2;
			yield return 3;
		}
		var routine = default(Finalizable);
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routine = r
		);

		stack.Push(new Effect());

		routine.MoveNext();
		var a = (int)(routine.Current ?? -1);
		routine.MoveNext();
		var b = (int)(routine.Current ?? -1);
		routine.MoveNext();
		var c = (int)(routine.Current ?? -1);

		Assert.AreEqual((1, 2, 3), (a, b, c));
	}

	[Test]
	public void CancelCallsRevertOnRunningEffect()
	{
		IEnumerator create() { yield return null; };
		var called = (a: false, b: false, c: false);
		var routine = default(Finalizable);
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routine = r
		);
		var effects = new Effect[] {
			new Effect(revert: () => called.a = true),
			new Effect(revert: () => called.b = true),
			new Effect(revert: () => called.c = true),
		};

		stack.Push(effects[0]);
		stack.Push(effects[1]);
		stack.Push(effects[2]);
		routine.MoveNext();
		routine.MoveNext();
		stack.Cancel();

		Assert.AreEqual((false, true, false), called);
	}

	[Test]
	public void CancelWithNoEffectDoesNotThrow()
	{
		IEnumerator create() { yield return null; };
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(_ => new Finalizable{ wrapped = create() });

		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void NewEventCallWhenPreviosRunCompleted()
	{
		IEnumerator create() { yield return null; };
		var routines = new List<Finalizable>();
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routines.Add(r)
		);

		stack.Push(new Effect());

		routines[0].MoveNext();
		routines[0].MoveNext();

		stack.Push(new Effect());

		Assert.AreNotSame(routines[0], routines[1]);
	}

	[Test]
	public void NewEventCallAfterCancel()
	{
		IEnumerator create() { yield return null; };
		var routines = new List<Finalizable>();
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onPull: r => routines.Add(r)
		);

		stack.Push(new Effect());
		stack.Cancel();
		stack.Push(new Effect());

		Assert.AreNotSame(routines[0], routines[1]);
	}

	[Test]
	public void NoOnCancelWhenNothingInStack()
	{
		IEnumerator create() { yield break; };
		var called = false;
		var factory = ScriptableObject.CreateInstance<DurationStackFactorySO>();
		var stack = factory.Create(
			_ => new Finalizable{ wrapped = create() },
			onCancel: _ => called = true
		);

		stack.Cancel();

		Assert.False(called);
	}
}
