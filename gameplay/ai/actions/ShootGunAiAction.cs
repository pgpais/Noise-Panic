using Gameplay.AI;
using Godot;
using System;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace Gameplay.AI.Actions;

[GlobalClass]
public partial class ShootGunAiAction : AiAction
{
	//TODO: implement aim controller for AI
	[Export] Gun gun;

	public override void Execute()
	{
		gun.Shoot();
	}
}
