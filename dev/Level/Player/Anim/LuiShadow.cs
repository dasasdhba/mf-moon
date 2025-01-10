using Component;
using Global;
using Godot;

namespace Level;

public partial class LuiShadow : ShadowCaster2D
{
    [ExportCategory("LuiShadow")]
    [Export]
    public PlayerRef Ref { get; set; }
    
    [Export]
    public Globalvar.PlayerState State { get; set; } = Globalvar.PlayerState.Lui;

    public override void _PhysicsProcess(double delta)
    {
        Emitting = Globalvar.Player.State == State
            && !Ref.MovementControl.IsDisabled() && !Ref.Body.IsOnFloor();
    }
}