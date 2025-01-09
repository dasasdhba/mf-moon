using System;
using Component;
using Godot;
using GodotTask;
using Utils;

namespace Level;

// TODO: refactor this to BounceMovement
// this would make it reusable for silver hammer, etc.

public partial class BeetMovement : Node, IFlipInit
{
    [ExportCategory("BeetMovement")]
    [Export]
    public CharaPlatformer2D Body { get; set; }
    
    [ExportGroup("InitSettings", "Init")]
    [Export]
    public float InitMoveSpeed { get ;set; } = 106.25f;
    
    [Export]
    public float InitJumpSpeed { get ;set; } = 250f;
    
    [ExportGroup("BounceSettings", "Bounce")]
    [Export]
    public int BounceCount { get; set; } = 3;
    
    [Export]
    public float BounceMoveSpeedMin { get ;set; } = 62.5f;
    
    [Export]
    public float BounceMoveSpeedMax { get ;set; } = 256.25f;
    
    [Export]
    public float BounceJumpSpeed { get ;set; } = 400f;
    
    [Signal]
    public delegate void BouncedEventHandler();
    
    private TaskCanceller GetThroughTask;
    
    public void FlipInit() => InitMoveSpeed *= -1f;

    public override void _Ready()
    {
        Body.SetMoveSpeed(InitMoveSpeed);
        Body.SetGravitySpeed(-InitJumpSpeed);
        
        Body.SignalWallCollided += () => TryBounce();
        Body.SignalFloorCollided += () => TryBounce();
        Body.SignalCeilingCollided += () =>
        {
            TryBounce();
            if (!IsBounceFinished()) GetThroughTask = Body.GetThrough();
        };
    }
    
    protected int BounceCounter { get; set; }
    
    public bool IsBounceFinished() => BounceCounter >= BounceCount;

    private int GetBounceDirection()
    {
        var last = Math.Sign(Body.MoveSpeed);
        return last == 0 ? (Mathe.Randf() < 0.5f ? 1 : -1) : -last;
    }
    
    private bool BounceCached;

    private async GDTaskVoid BounceRestore()
    {
        await GDTask.WaitForPhysicsProcess();
        BounceCached = false;
    }

    public bool TryBounce()
    {
        if (IsBounceFinished() || BounceCached) return false;
        
        BounceCached = true;
        BounceRestore().Forget();
        
        Body.SetMoveSpeed(GetBounceDirection() *
            Mathe.RandfRange(BounceMoveSpeedMin, BounceMoveSpeedMax));
        Body.SetGravitySpeed(-BounceJumpSpeed);
        
        BounceCounter++;
        if (IsBounceFinished())
        {
            Body.CollisionMask = 0;
        }
        
        EmitSignal(SignalName.Bounced);
        return true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Body.IsInWater()) return;

        if (GetThroughTask != null)
        {
            GetThroughTask.Cancel();
            GetThroughTask = null;
        }
        
        Body.CollisionMask = 0;
    }
}