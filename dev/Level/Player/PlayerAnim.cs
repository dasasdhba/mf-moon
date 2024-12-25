using System;
using Global;
using Godot;
using Utils;

namespace Level;

/// <summary>
/// general player animation control.
/// </summary>
public partial class PlayerAnim : AnimGroup2D
{
    [ExportCategory("PlayerAnim")]
    [Export]
    public float MoveMinSpeedScale { get ;set; } = 0.5f;
    
    [Export]
    public float MoveMaxSpeedScale { get ;set; } = 2f;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }

    public override void _EnterTree()
    {
        Ref.Jump.SignalSwum += () => Play("Swim");
        Ref.Jump.SignalJumpedOutWater += () => Play("Swim");
        SignalAnimationFinished += () =>
        {
            if (GetAnimation() == "Swim")
            {
                Play("Dive");
            }
        };
        
        CurrentSprite = Globalvar.Player.State.ToString();
        
        this.AddPhysicsProcess(Process);
    }
    
    private const double LaunchTime = 0.05d;
    private double LaunchTimer = 0d;
    
    public void PlayLaunch() => LaunchTimer = LaunchTime;

    protected void Process(double delta)
    {
        var body = Ref.Body;
        var walk = Ref.Walk;
        
        CurrentSprite = Globalvar.Player.State.ToString();
        SpeedScale = 1f;
        
        if (LaunchTimer > 0d) LaunchTimer -= delta;

        if (Ref.Shape.IsStucked())
        {
            Play("Idle");
            return;
        }

        if (walk.IsCrouching())
        {
            Play("Crouch");
            return;
        }

        if (LaunchTimer > 0d && body.IsOnFloor())
        {
            Play("Launch");
            return;
        }
        
        // general animation
        
        var speed = body.GetLastMoveSpeed();
        if (speed != 0f) FlipH = speed < 0f;

        if (body.IsOnFloor())
        {
            if (speed != 0f)
            {
                SpeedScale = Mathf.Lerp(MoveMinSpeedScale,
                 MoveMaxSpeedScale, 
                 Math.Abs(speed) / walk.RunSpeed);
                Play("Walk");
            }
            else
            {
                Play("Idle");
            }
        }
        else if (!body.IsInWater())
        {
            Play("Jump");
        }
        else if (GetAnimation() != "Swim")
        {
            Play("Dive");
        }
    }
}