using System;
using Component;
using Godot;
using GodotTask;
using Utils;

namespace Level;

public partial class BounceMovement : Node, IFlipInit
{
    [ExportCategory("BounceMovement")]
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
    
    public void FlipInit() => InitMoveSpeed *= -1f;
    
    private TaskCanceller GetThroughTask;
    protected void CancelGetThrough()
    {
        if (GetThroughTask == null) return;
        
        GetThroughTask.Cancel();
        GetThroughTask = null;
    }

    public BounceMovement() : base()
    {
        Ready += () =>
        {    
            Body.SetMoveSpeed(InitMoveSpeed);
            Body.SetGravitySpeed(-InitJumpSpeed);
        
            Body.SignalWallCollided += (dir) => TryBounce();
            Body.SignalFloorCollided += () => TryBounce();
            Body.SignalCeilingCollided += () =>
            {
                TryBounce();
                if (!IsBounceFinished()) GetThroughTask = Body.GetThrough();
            };
        };
    }
    
    private int BounceCounter;
    public bool IsBounceFinished() => BounceCounter >= BounceCount;

    private int GetBounceDirection()
    {
        var last = Math.Sign(Body.GetMoveSpeed());
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
}