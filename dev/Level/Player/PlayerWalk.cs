using System;
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
    public bool AllowCrouch { get; set; } = true;

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

    public override void _PhysicsProcess(double delta)
    {
        var input = Ref.Input;
        var body = Ref.Body;
        
        var dir = (input.GetKey("Right").Pressed ? 1 : 0)
                  - (input.GetKey("Left").Pressed ? 1 : 0);
        var last = body.GetLastMoveSpeed();
        var lastDir = Math.Sign(last);
        
        Crouching = AllowCrouch && body.IsOnFloor() && input.GetKey("Down").Pressed;
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
        Move.MaxSpeed = input.GetKey("Fire").Pressed ? RunSpeed : WalkSpeed;

        if (Move.IsMoving())
        {
            if (last < MinSpeed)
            {
                body.SetMoveSpeed(last + MinSpeed * lastDir);
            }
        }
        
        if (body.IsReallyOnWall()) body.SetMoveSpeed(0f);
    }
}