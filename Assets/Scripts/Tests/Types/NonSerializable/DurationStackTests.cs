using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DurationStackTests : TestCollection
{
	private class MockEffectRoutineFactory : IEffectRoutineFactory
	{
		public Func<Effect, Finalizable> create;
		public Finalizable Create(Effect effect) => this.create(effect);
	}

	[Test]
	public void RoutineFromEffect()
	{
		var called = false;
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create(Effect effect) {
			effect.Apply();
			yield break;
		}
		factory.create = e => new Finalizable{ wrapped = create(e) };
		stack.Factory = factory;
		stack.OnPull += r => r.MoveNext();

		stack.Push(new Effect(apply: () => called = true));

		Assert.True(called);
	}

	[Test]
	public void CancelCallWithPushRoutine()
	{
		var routines = (onPush: default(Finalizable), onCancel: default(Finalizable));
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routines.onPush = r;
		stack.OnCancel += r => routines.onCancel = r;

		stack.Push(new Effect());
		stack.Cancel();

		Assert.AreSame(routines.onPush, routines.onCancel);
	}

	[Test]
	public void NoOnCancelWhenNoCancel()
	{
		var called = false;
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnCancel += _ => called = true;

		stack.Push(new Effect());

		Assert.False(called);
	}

	[Test]
	public void EmptyPushDoesNotThrow()
	{
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		Assert.DoesNotThrow(() => stack.Push(new Effect()));
	}

	[Test]
	public void EmptyCancelDoesNotThrow()
	{
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		stack.Push(new Effect());
		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void Effects()
	{
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		var effects = new Effect[] {
			new Effect(),
			new Effect(),
		};
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		stack.Push(effects[0]);
		stack.Push(effects[1]);

		CollectionAssert.AreEqual(effects, stack.Effects);
	}

	[Test]
	public void CancelClearsEffects()
	{
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		var effects = new Effect[] {
			new Effect(),
			new Effect(),
		};
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		stack.Push(effects[0]);
		stack.Push(effects[1]);
		stack.Cancel();

		Assert.IsEmpty(stack.Effects);
	}

	[Test]
	public void OnPushOnlyForFirst()
	{
		var called = 0;
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() {
			yield return null;
			yield return null;
		};
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += _ => ++called;

		stack.Push(new Effect());
		stack.Push(new Effect());

		Assert.AreEqual(1, called);
	}

	[Test]
	public void FirstRoutinesAlsoYieldsSubsequentEffects()
	{
		var called = 0;
		var routine = default(Finalizable);
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() {
			++called;
			yield return null;
			++called;
			yield return null;
			++called;
		};
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routine = r;

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
		var routine = default(Finalizable);
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() {
			yield return null;
			yield return null;
			yield return null;
		}
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routine = r;

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
		var routine = default(Finalizable);
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() {
			yield return 1;
			yield return 2;
			yield return 3;
		}
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routine = r;

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
		var called = (a: false, b: false, c: false);
		var routine = default(Finalizable);
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		var effects = new Effect[] {
			new Effect(revert: () => called.a = true),
			new Effect(revert: () => called.b = true),
			new Effect(revert: () => called.c = true),
		};
		IEnumerator create() { yield return null; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routine = r;

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
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield return null; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void NewEventCallWhenPreviosRunCompleted()
	{
		var routines = new List<Finalizable>();
		var stack = new DurationStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield return null; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnPull += r => routines.Add(r);

		stack.Push(new Effect());

		routines[0].MoveNext();
		routines[0].MoveNext();

		stack.Push(new Effect());

		Assert.AreNotSame(routines[0], routines[1]);
	}
}
