using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class GameObjectEvent : UnityEvent<GameObject> {}
[Serializable] public class Vector3Event : UnityEvent<Vector3> {}
