using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseConditionManagerMBTests : TestCollection
{
	private class MockEffectRoutineFactory : IEffectRoutineFactory
	{
		public Func<Effect, Finalizable> create = MockEffectRoutineFactory.Empty;

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

	private class MockConditionManagerMB : BaseConditionManagerMB<MockEffectRoutineFactory, MockStacking> {}

	[UnityTest]
	public IEnumerator StackFromCreatedRoutine()
	{
		IEnumerator get() { yield return new WaitForFixedUpdate(); }
		var called = default(Finalizable);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		var routine = new Finalizable { wrapped = get() };

		manager.effectRoutineFactory.create = _ => routine;
		manager.effectRoutineStacking.add = (r, _, __) => called = r;

		yield return new WaitForEndOfFrame();

		manager.Add(effect);

		Assert.AreSame(routine, called);
	}

	[UnityTest]
	public IEnumerator RoutineListNotNull()
	{
		var called = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => called = l;

		manager.Add(effect);

		Assert.NotNull(called);
	}

	[UnityTest]
	public IEnumerator RoutineListConsistant()
	{
		var calledA = default(List<Finalizable>);
		var calledB = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => calledA = l;

		manager.Add(effect);

		manager.effectRoutineStacking.add = (_, l, __) => calledB = l;

		manager.Add(effect);

		Assert.AreSame(calledA, calledB);
	}

	[UnityTest]
	public IEnumerator RoutineListTagSpezific()
	{
		var calledA = default(List<Finalizable>);
		var calledB = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.effectRoutineStacking.add = (_, l, __) => calledA = l;

		manager.Add(effect);

		manager.effectRoutineStacking.add = (_, l, __) => calledB = l;

		effect.tag = EffectTag.Heat;
		manager.Add(effect);

		Assert.AreNotSame(calledA, calledB);
	}

	[UnityTest]
	public IEnumerator RunNewRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
			++called;
			yield return new WaitForFixedUpdate();
			++called;
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, __, run) => run(r);

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RunOnlyNewRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, __, run) => run(r);

		manager.Add(effect);

		manager.effectRoutineStacking.add = (_, __, ___) => { };

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RemoveFinishedCoroutine()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			routines = l;
			run(r);
		};

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(routines);
	}

	[UnityTest]
	public IEnumerator CancelCoroutines()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect());

		yield return new WaitForFixedUpdate();

		manager.Cancel(EffectTag.Physical);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator CancelCoroutinesTagSpecific()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect{ tag = EffectTag.Physical });
		manager.Add(new Effect{ tag = EffectTag.Heat });

		yield return new WaitForFixedUpdate();

		manager.Cancel(EffectTag.Physical);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator CancelRemovesCoroutines()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		IEnumerator get() {
			while (true) {
				yield return new WaitForFixedUpdate();
			}
		}

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			routines = l;
			l.Add(r);
			run(r);
		};

		yield return new WaitForEndOfFrame();

		manager.Add(new Effect());

		manager.Cancel(EffectTag.Physical);

		Assert.IsEmpty(routines);
	}

	[UnityTest]
	public IEnumerator GetEffects()
	{
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effectA = new Effect();
		var effectB = new Effect();
		var effectC = new Effect{ tag = EffectTag.Heat };

		yield return new WaitForFixedUpdate();

		manager.Add(effectA);
		manager.Add(effectB);
		manager.Add(effectC);

		CollectionAssert.AreEqual(
			new Effect[] { effectA, effectB },
			manager.GetEffects(EffectTag.Physical)
		);
	}

	[UnityTest]
	public IEnumerator RemoveEffectAfterEffect()
	{
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get() {
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get() };
		manager.effectRoutineStacking.add = (r, l, run) => {
			l.Add(r);
			run(r);
		};

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(manager.GetEffects(EffectTag.Physical));
	}


	[UnityTest]
	public IEnumerator CancelRevertsEffects()
	{
		var called = false;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect(revert: () => called = true){ duration = 1f };
		IEnumerator get(int count) {
			for (int i = 0; i < count; ++i) {
				yield return new WaitForFixedUpdate();
			}
		}
		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get(100) };

		yield return new WaitForEndOfFrame();

		manager.Add(effect);

		yield return new WaitForFixedUpdate();

		manager.Cancel(EffectTag.Physical);

		Assert.True(called);
	}

	[UnityTest]
	public IEnumerator CancelRemovesEffects()
	{
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();

		yield return new WaitForEndOfFrame();

		manager.Add(effect);

		manager.Cancel(EffectTag.Physical);

		Assert.IsEmpty(manager.GetEffects(EffectTag.Physical));
	}

	[UnityTest]
	public IEnumerator RunProperRoutine()
	{
		var called = 0;
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get(int count) {
			for (int i = 0; i < count; ++i) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get(0) };
		manager.effectRoutineStacking.add = (_, __, run) => run(new Finalizable{ wrapped = get(2) });

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator RemoveProperRoutine()
	{
		var routines = default(List<Finalizable>);
		var manager = new GameObject("obj").AddComponent<MockConditionManagerMB>();
		var effect = new Effect();
		IEnumerator get(int count) {
			for (int i = 0; i < count; ++i) {
				yield return new WaitForFixedUpdate();
			}
		}

		yield return new WaitForEndOfFrame();

		manager.effectRoutineFactory.create = _ => new Finalizable { wrapped = get(0) };
		manager.effectRoutineStacking.add = (_, l, run) => {
			Finalizable proper = new Finalizable{ wrapped = get(2) };
			l.Add(proper);
			run(proper);
			routines = l;
		};

		manager.Add(effect);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.IsEmpty(routines);
	}
}
