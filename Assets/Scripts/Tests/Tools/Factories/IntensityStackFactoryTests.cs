using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class IntensityStackFactoryTests : TestCollection
{
	[Test]
	public void RoutineFromEffect() {
		IEnumerator create(Effect effect) {
			effect.Apply();
			yield break;
		}
		var called = false;
		var stack = IntensityStackFactory.Create(
			effectToRoutine: e => new Finalizable(create(e)),
			onPull: r => r.MoveNext()
		);

		stack.Push(new Effect(apply: () => called = true));

		Assert.True(called);
	}

	[Test]
	public void CancelCallWithPushRoutine() {
		IEnumerator create() { yield break; };
		var routines = (onPush: default(Finalizable), onCancel: default(Finalizable));
		var stack = IntensityStackFactory.Create(
			effectToRoutine: _ => new Finalizable(create()),
			onPull: r => routines.onPush = r,
			onCancel: r => routines.onCancel = r
		);

		stack.Push(new Effect());
		stack.Cancel();

		Assert.AreSame(routines.onPush, routines.onCancel);
	}

	[Test]
	public void EmptyOnPullDoesNotThrow() {
		IEnumerator create() { yield break; };
		var stack = IntensityStackFactory.Create(effectToRoutine: _ => new Finalizable(create()));

		Assert.DoesNotThrow(() => stack.Push(new Effect()));
	}

	[Test]
	public void EmptyCancelDoesNotThrow() {
		IEnumerator create() { yield break; };
		var stack = IntensityStackFactory.Create(effectToRoutine: _ => new Finalizable(create()));

		stack.Push(new Effect());
		Assert.DoesNotThrow(() => stack.Cancel());
	}

	[Test]
	public void NoOnCancelWhenNoCancel() {
		IEnumerator create() { yield break; };
		var called = false;
		var stack = IntensityStackFactory.Create(
			effectToRoutine: _ => new Finalizable(create()),
			onCancel: _ => called = true
		);

		stack.Push(new Effect());

		Assert.False(called);
	}

	[Test]
	public void GetEffects() {
		IEnumerator create() { yield break; };
		var stack = IntensityStackFactory.Create(effectToRoutine: _ => new Finalizable(create()));
		var effects = new Effect[] {
			new Effect(),
			new Effect(),
		};

		stack.Push(effects[0]);
		stack.Push(effects[1]);

		CollectionAssert.AreEqual(effects, stack.Effects);
	}

	[Test]
	public void CancelCallsRevert() {
		IEnumerator create() { yield break; };
		var called = 0;
		var stack = IntensityStackFactory.Create(effectToRoutine: _ => new Finalizable(create()));
		var effects = new Effect[] {
			new Effect(revert: () => ++called),
			new Effect(revert: () => ++called),
		};

		stack.Push(effects[0]);
		stack.Push(effects[1]);
		stack.Cancel();

		Assert.AreEqual(2, called);
	}

	[Test]
	public void CancelClearsEffects() {
		IEnumerator create() { yield break; };
		var stack = IntensityStackFactory.Create(effectToRoutine: _ => new Finalizable(create()));
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
	public void FinishedEffectRemovedFromEffects() {
		IEnumerator create() {
			yield return null;
			yield return null;
			yield return null;
		}
		var routine = default(Finalizable);
		var stack = IntensityStackFactory.Create(
			effectToRoutine: _ => new Finalizable(create()),
			onPull: r => routine = r
		);

		stack.Push(new Effect());

		routine!.MoveNext();
		routine.MoveNext();
		routine.MoveNext();
		routine.MoveNext();

		Assert.IsEmpty(stack.Effects);
	}
}
