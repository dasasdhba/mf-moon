using System.Collections.Generic;
using Component;
using Godot;
using Utils;

namespace Level;

public partial class PlayerProjectile : PlayerAttack
{
    [ExportCategory("PlayerProjectile")]
    [Export]
    public PackedScene ProjectileScene { get; set; }
    public AsyncLoader<Node2D> ProjectileLoader { get; set; }
    
    [Export]
    public MarkerFlipSync GeneratePoint { get ;set; }
    
    [Export]
    public int MaxCount { get; set; } = 2;
    
    [Signal]
    public delegate void FiredEventHandler(Node2D projectile);
    
    private List<Node> Projectiles = [];

    public PlayerProjectile() : base()
    {
        TreeEntered += () =>
        {
            ProjectileLoader = new(this, ProjectileScene, MaxCount);
        };
        
        SignalFired += p =>
        {
            Ref.Anim.PlayLaunch();
        };
    }

    protected override void AttackProcess(double delta)
    {
        if (Ref.Input.GetKey("Fire").JustPressed && !Ref.Walk.IsCrouching()
             && Projectiles.Count < MaxCount)
        {
            var p = ProjectileLoader.Create();
            if (Ref.Anim.FlipH) p.TryInitFlipAll();
            p.TreeEntered += () => p.GlobalPosition = GeneratePoint.GlobalPosition;
            Projectiles.Add(p);
            p.TreeExited += () => Projectiles.Remove(p);
            EmitSignal(SignalName.Fired, p);
            Ref.Body.AddSibling(p);
        }
    }
}