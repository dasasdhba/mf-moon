using System;
using Global;
using Godot;
using Utils;

namespace Level;

/// <summary>
/// player walk and crouch logic
/// </summary>
public partial class PlayerWalk : Node
{
    [ExportCategory("PlayerWalk")]
    [ExportGroup("Settings")]
    [Export]
    public float MinSpeed { get; set; } = 50f;

    [Export]
    public float WalkSpeed { get; set; } = 218.75f;

    [Export]
    public float RunSpeed { get; set; } = 375f;

    [Export]
    public float AccSpeed { get; set; } = 312.5f;

    [Export]
    public float DecSpeed { get; set; } = 312.5f;

    [Export]
    public float TurnDec { get; set; } = 1250f;

    [Export]
    public float CrouchDec { get; set; } = 625f;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get; set; }

    public PlatformerMove2D Move { get; set; }
    
    public override void _EnterTree()
    {
        Move = new()
        {
            Platformer = Ref.Body,
            MoveMode = PlatformerMove2D.PlatformerMove2DMoveMode.Accelerate,
            TurnMode = PlatformerMove2D.PlatformerMove2DTurnMode.None,
        };
        Move.BindParent(this);
        AddChild(Move);
    }
    
    protected bool Crouching { get; set; } = false;
    public bool IsCrouching() => Crouching;
    public static bool IsAllowCrouch() => Globalvar.Player.State != Globalvar.PlayerState.Small;

    public override void _PhysicsProcess(double delta)
    {
        var disabled = Ref.MovementControl.IsDisabled();
        Move.Disabled = disabled;
        if (disabled) return;
    
        var input = Ref.Input;
        var body = Ref.Body;
        
        var dir = (input.GetKey("Right").Pressed ? 1 : 0)
                  - (input.GetKey("Left").Pressed ? 1 : 0);
        var last = body.GetLastMoveSpeed();
        var lastDir = Math.Sign(last);
        
        Crouching = IsAllowCrouch() && body.IsOnFloor() && input.GetKey("Down").Pressed;
        if (Crouching)
        {
            Move.Direction = 0;
            
            var dec = CrouchDec;
            if (dir != lastDir) dec += DecSpeed;
            Move.DecSpeed = dec;
            
            return;
        }
        
        Move.Direction = dir;
        Move.AccSpeed = AccSpeed;
        Move.DecSpeed = DecSpeed;
        Move.TurnDec = TurnDec;
        Move.MaxSpeed = input.GetKey("Fire").Pressed && !body.IsInWater()
             ? RunSpeed : WalkSpeed;

        if (Move.IsMoving())
        {
            if (Math.Abs(last) < MinSpeed)
            {
                Move.Speed += MinSpeed * dir;
            }
        }

        if (body.IsReallyOnWall())
        {
            Move.Speed = 0f;
            if (body.IsOnFloor()) Move.Direction = 0; // prevent animation issue
        }
    }
}