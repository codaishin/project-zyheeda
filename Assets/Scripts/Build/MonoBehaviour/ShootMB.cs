using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileLauncher : BaseProjectileLauncher<ProjectilePathing> {}

public class ShootMB : BaseSkillMB<ProjectileLauncher> { }
