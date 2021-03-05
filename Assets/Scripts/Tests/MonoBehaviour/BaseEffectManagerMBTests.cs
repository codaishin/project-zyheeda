using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEffectManagerMBTests : TestCollection
{
	private class MockEffectRoutineCreator : IEffectRoutineCreator
	{
		public Func<Effect, Finalizable> create = MockEffectRoutineCreator.Empty;

		private static Finalizable Empty(Effect effect)
		{
			IEnumerator get() { yield break; }
			return new Finalizable { wrapped = get() };
		}

		public Finalizable Create(Effect effect) => this.create(effect);
	}

	private class MockStacking : IEffectRoutineStacking
	{
		public Action<Finalizable, List<Finalizable>, Action<Finalizable>> add = (_, __, ___) => { };

		public void Add(Finalizable effect, List<Finalizable> stackRoutines, Action<Finalizable> onAdd) =>
			this.add(effect, stackRoutines, onAdd);
	}

	private class MockEffectManagerMB : BaseEffectManagerMB<MockEffectRoutineCreator, MockStacking> {}

	[UnityTest]
	public IEnumerator StackFromCreatedRoutine()
	{
		IEnumerator get() { yield return new WaitForFixedUpdate(); }
		var called = default(Finalizable);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		var routine = new Finalizable { wrapped = get() };

		manager.effectRoutineCreator.create = _ => routine;
		manager.effectRoutineStacking.add = (r, _, __) => called = r;

		yield return new WaitForEndOfFrame();

		manager.Add(effect, EffectTag.Default);

		Assert.AreSame(routine, called);
	}

	[UnityTest]
	public IEnumerator RoutineListNotNull()
	{
		var called = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => called = l;

		manager.Add(effect, EffectTag.Default);

		Assert.NotNull(called);
	}

	[UnityTest]
	public IEnumerator RoutineListConsistant()
	{
		var calledA = default(List<Finalizable>);
		var calledB = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => calledA = l;

		manager.Add(effect, EffectTag.Default);

		manager.effectRoutineStacking.add = (_, l, __) => calledB = l;

		manager.Add(effect, EffectTag.Default);

		Assert.AreSame(calledA, calledB);
	}

	[UnityTest]
	public IEnumerator RoutineListTagSpezific()
	{
		var calledA = default(List<Finalizable>);
		var calledB = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => calledA = l;

		manager.Add(effect, EffectTag.Default);

		manager.effectRoutineStacking.add = (_, l, __) => calledB = l;

		manager.Add(effect, EffectTag.Heat);

		Assert.AreNotSame(calledA, calledB);
	}

	[UnityTest]
	public IEnumerator RunNewRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
			++called;
			yield return new WaitForFixedUpdate();
			++called;
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, __, run) => run(r);

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RunOnlyNewRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, __, run) => run(r);

		manager.Add(effect, EffectTag.Default);

		manager.effectRoutineStacking.add = (_, __, ___) => { };

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RemoveFinishedCoroutine()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			routines = l;
			run(r);
		};

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(routines);
	}

	[UnityTest]
	public IEnumerator CancelCoroutines()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect(), EffectTag.Default);

		yield return new WaitForFixedUpdate();

		manager.Cancel(EffectTag.Default);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator CancelCoroutinesTagSpecific()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect(), EffectTag.Default);
		manager.Add(new Effect(), EffectTag.Heat);

		yield return new WaitForFixedUpdate();

		manager.Cancel(EffectTag.Default);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator CancelRemovesCoroutines()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
			}
		}

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			routines = l;
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect(), EffectTag.Default);

		manager.Cancel(EffectTag.Default);

		Assert.IsEmpty(routines);
	}

	[UnityTest]
	public IEnumerator GetEffects()
	{
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effectA = new Effect();
		var effectB = new Effect();
		var effectC = new Effect();

		yield return new WaitForFixedUpdate();

		manager.Add(effectA, EffectTag.Default);
		manager.Add(effectB, EffectTag.Default);
		manager.Add(effectC, EffectTag.Heat);

		CollectionAssert.AreEqual(
			new Effect[] { effectA, effectB },
			manager.GetEffects(EffectTag.Default)
		);
	}

	[UnityTest]
	public IEnumerator RemoveEffectAfterEffect()
	{
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(manager.GetEffects(EffectTag.Default));
	}


	[UnityTest]
	public IEnumerator CancelRevertsEffects()
	{
		var called = false;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		effect.OnRevert += () => called = true;
		effect.Apply();

		yield return new WaitForEndOfFrame();

		manager.Add(effect, EffectTag.Default);

		manager.Cancel(EffectTag.Default);

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator CancelRemovesEffects()
	{
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.Add(effect, EffectTag.Default);

		manager.Cancel(EffectTag.Default);

		Assert.IsEmpty(manager.GetEffects(EffectTag.Default));
	}

	[UnityTest]
	public IEnumerator RunProperRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get(int count) {
			for (int i = 0; i < count; ++i) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get(0) };
		manager.effectRoutineStacking.add = (_, __, run) => run(new Finalizable{ wrapped = get(2) });

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RemoveProperRoutine()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockEffectManagerMB>();
		var effect = new Effect();
		IEnumerator get(int count) {
			for (int i = 0; i < count; ++i) {
				yield return new WaitForFixedUpdate();
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineCreator.create = _ => new Finalizable { wrapped = get(0) };
		manager.effectRoutineStacking.add = (_, l, run) => {
			Finalizable proper = new Finalizable{ wrapped = get(2) };
			l.Add(proper);
			run(proper);
			routines = l;
		};

		manager.Add(effect, EffectTag.Default);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(routines);
	}
}
