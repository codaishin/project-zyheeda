using System;
using System.Collections;
using NUnit.Framework;

public class IntensityStackTests : TestCollection
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
		var stack = new IntensityStack<MockEffectRoutineFactory>();
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
		var stack = new IntensityStack<MockEffectRoutineFactory>();
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
	public void EmptyPushDoesNotThrow()
	{
		var stack = new IntensityStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		Assert.DoesNotThrow(() => stack.Push(new Effect()));
	}

	[Test]
	public void EmptyCancelDoesNotThrow()
	{
		var stack = new IntensityStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		stack.Push(new Effect());
		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void NoOnCancelWhenNoCancel()
	{
		var called = false;
		var stack = new IntensityStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;
		stack.OnCancel += _ => called = true;

		stack.Push(new Effect());

		Assert.False(called);
	}

	[Test]
	public void Effects()
	{
		var stack = new IntensityStack<MockEffectRoutineFactory>();
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
	public void CancelCallsRevert()
	{
		var called = 0;
		var stack = new IntensityStack<MockEffectRoutineFactory>();
		var factory = new MockEffectRoutineFactory();
		var effects = new Effect[] {
			new Effect(revert: () => ++called),
			new Effect(revert: () => ++called),
		};
		IEnumerator create() { yield break; };
		factory.create = e => new Finalizable{ wrapped = create() };
		stack.Factory = factory;

		stack.Push(effects[0]);
		stack.Push(effects[1]);
		stack.Cancel();

		Assert.AreEqual(2, called);
	}

	[Test]
	public void CancelClearsEffects()
	{
		var stack = new IntensityStack<MockEffectRoutineFactory>();
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
	public void FinishedEffectRemovedFromEffects()
	{
		var routine = default(Finalizable);
		var stack = new IntensityStack<MockEffectRoutineFactory>();
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
}
