using Godot;

using RandomDungeons.Nodes.Components;
using RandomDungeons.Nodes.Elements.Enemies;
using RandomDungeons.StateMachines;

namespace RandomDungeons.Nodes.Elements
{
    public class BreakablePot : BaseEnemy
    {
        protected override Node2D Visuals() => GetNode<Node2D>("%Visuals");
        protected override HurtBox Hurtbox() => GetNode<HurtBox>("%HurtBox");
        protected override IState InitialState() => null;
    }
}
