using System;
using Global;
using Godot;
using Utils;

namespace Level;

public partial class PlayerTransform : AnimSprite2D
{
    [ExportCategory("PlayerTransform")]
    [Export]
    public float TransformTime { get ;set; } = 1f;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get; set; }

    [Signal]
    public delegate void PowerUpEventHandler();
    
    [Signal]
    public delegate void PowerDownEventHandler();
    
    private Globalvar.PlayerState LastState;
    private double TransformTimer = 0d;

    public void ClearTransform()
    {
        if (TransformTimer > 0d)
        {
            TransformTimer = 0d;
            Ref.Anim.Show();
            Hide();
        }
    }

    public override void _EnterTree()
    {
        LastState = Globalvar.Player.State;
        Hide();
        
        this.AddPhysicsProcess(Process);
    }

    private void Process(double delta)
    {
        if (LastState != Globalvar.Player.State)
        {
            var intState = (int)Globalvar.Player.State;
            if (intState > Math.Min(1, (int)LastState))
            {
                var anim = intState > 1 ? Globalvar.Player.State.ToString() : "Small";
                if (!SpriteFrames.HasAnimation(anim)) anim = Globalvar.PlayerState.Fire.ToString();
                Play(anim);
                
                TransformTimer = TransformTime;
                Ref.Anim.Hide();
                Show();
                EmitSignal(SignalName.PowerUp);
            }
            else
            {
                Play("Small");
                TransformTimer = TransformTime;
                Ref.Anim.Hide();
                Show();
                EmitSignal(SignalName.PowerDown);
            }
            
            LastState = Globalvar.Player.State;
        }

        if (TransformTimer > 0d)
        {
            TransformTimer -= delta;
            FlipH = Ref.Anim.FlipH;
            if (TransformTimer <= 0d)
            {
                Ref.Anim.Show();
                Hide();
            }
        }
    }
}