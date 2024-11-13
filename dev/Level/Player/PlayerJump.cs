using System;
using Global;
using Godot;
using Utils;

namespace Level;

public partial class PlayerJump : Node
{
    [ExportCategory("PlayerJump")]
    [ExportGroup("Settings")]
    [Export]
    public float JumpSpeed { get ;set; } = 650f;
    
    [Export]
    public float SwimSpeed { get ;set; } = 150f;
    
    [Export]
    public float JumpOutWaterSpeed { get ;set; } = 450f;
    
    [Export]
    public float JumpOutWaterMargin { get ;set; } = 24f;
    
    [Export]
    public float JumpBoostIdle { get ;set; } = 1000f;
    
    [Export]
    public float JumpBoostMoving { get ;set; } = 1250f;
    
    [Export]
    public float JumpBoostLui { get ;set; } = 1500f;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }
    
    [Signal]
    public delegate void JumpedEventHandler();
    
    [Signal]
    public delegate void SwumEventHandler();
    
    [Signal]
    public delegate void JumpedOutWaterEventHandler();

    public override void _EnterTree()
    {
        this.AddPhysicsProcess(JumpProcess);
    }

    protected void JumpProcess(double delta)
    {
        var input = Ref.Input;
        var body = Ref.Body;
        var walk = Ref.Walk;
        
        // jump and swim

        if (input.IsKeyPressed("Jump", true))
        {
            if (!body.IsInWater())
            {
                if (body.IsOnFloor())
                {
                    input.SetKeyBuffered("Jump");
                    body.SetGravitySpeed(-JumpSpeed);
                    EmitSignal(SignalName.Jumped);
                }
            }
            else
            {
                input.SetKeyBuffered("Jump");
                if (body.IsInWater(JumpOutWaterMargin * Vector2.Up))
                {
                    body.SetGravitySpeed(-SwimSpeed);
                    EmitSignal(SignalName.Swum);
                }
                else
                {
                    body.SetGravitySpeed(-JumpOutWaterSpeed);
                    EmitSignal(SignalName.JumpedOutWater);
                }
            }
        }
        
        // boost

        if (!body.IsInWater() && body.Gravity < 0f && input.GetKey("Jump").Pressed)
        {
            var isLui = Globalvar.Player.State == Globalvar.PlayerState.Lui;
            
            float boost;
            if (isLui) boost = JumpBoostLui;
            else if (Math.Abs(body.GetLastMoveSpeed()) < walk.MinSpeed) boost = JumpBoostIdle;
            else boost = JumpBoostMoving;
            
            body.Gravity -= (float)(boost * delta);
        }
    }
}