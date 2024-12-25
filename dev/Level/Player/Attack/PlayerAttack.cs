using Global;
using Godot;
using Utils;

namespace Level;

public partial class PlayerAttack : Node
{
    [ExportCategory("PlayerAttack")]
    [Export]
    public Globalvar.PlayerState State { get ;set; }
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }

    public PlayerAttack() : base()
    {
        TreeEntered += () =>
        {
            this.AddPhysicsProcess(delta =>
            {    
                if (Ref.MovementControl.IsDisabled()
                    || Globalvar.Player.State != State) return;
                
                AttackProcess(delta);
            });
        };
    }
    
    protected virtual void AttackProcess(double delta) { }
}