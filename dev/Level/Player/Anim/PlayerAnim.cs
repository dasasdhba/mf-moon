using System;
using Component;
using Global;
using Godot;
using GodotTask;
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
        CurrentSprite = Globalvar.Player.State.ToString();
        
        this.AddPhysicsProcess(Process);
    }

    public override void _Ready()
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
        
        Ref.Hurt.SignalPowerDown += () => PlayHurt().Forget();
    }

    private double HurtFlashInterval = 0.03d;
    private float HurtAlpha = 0f;
    public async GDTaskVoid PlayHurt()
    {
        var timer = new STimer(HurtFlashInterval);
        var flash = false;
        await Async.DelegateProcess(this, delta =>
        {
            if (timer.Update(delta))
            {
                Ref.Body.Modulate = Ref.Body.Modulate with { A = flash ? HurtAlpha : 1f };
                flash = !flash;
            }
            return !Ref.Hurt.IsInHurt();
        });
        
        Ref.Body.Modulate = Ref.Body.Modulate with { A = 1f };
    }
    
    // launch animation is connected by PlayerProjectile
    
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