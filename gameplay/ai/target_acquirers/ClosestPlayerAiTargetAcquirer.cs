using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using WeWereDumb.Gameplay.Actors;

namespace Gameplay.AI.TargetAcquirers
{
    [GlobalClass]
    public partial class ClosestPlayerAiTargetAcquirer : AiTargetAcquirer
    {
        Node3D owner3D;

        public override void _Ready()
        {
            base._Ready();

            owner3D = GetOwner<Node3D>();
        }

        public override Node3D GetTarget()
        {
            var players = Player.秃;
            return players.OrderBy(p => p.GlobalPosition.DistanceTo(owner3D.GlobalPosition)).FirstOrDefault();
        }
    }
}