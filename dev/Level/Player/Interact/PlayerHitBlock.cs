using Global;
using Godot;

namespace Level;

public partial class PlayerHitBlock : BlockHitPlatformer
{
    [ExportCategory("PlayerHitBlock")]
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        Disabled = Ref.MovementControl.IsDisabled();
        JumpHardness = Globalvar.Player.State == Globalvar.PlayerState.Small ? -1 : 0;
    }
}