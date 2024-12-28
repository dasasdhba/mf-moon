using Global;
using Godot;
using Utils;

namespace Level;

public partial class PlayerHurt : Node
{
    [ExportCategory("PlayerHurt")]
    [Export]
    public double HurtTime { get; set; } = 2d;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get; set; }
    
    [Signal]
    public delegate void PowerDownEventHandler();
    
    [Signal]
    public delegate void DeadEventHandler();

    private double HurtTimer = 0d;
    public bool IsInHurt() => HurtTimer > 0d;
    public void ClearHurt() => HurtTimer = 0d;

    public PlayerHurt() : base()
    {
        TreeEntered += () =>
        {
            this.AddPhysicsProcess(delta =>
            {
                if (Ref.InteractionControl.IsDisabled()) return;
                
                if (HurtTimer > 0d) HurtTimer -= delta;
            });
        };
    }

    public bool TryHurt(EnemyRef enemy)
    {
        if (enemy.Critical)
        {
            if (
                (!IsInHurt() || enemy.IgnorePlayerHurt) &&
                (!Ref.Star.IsInStar() || enemy.IgnorePlayerStar)
            )
            {
                Die();
                return true;
            }
            
            return false;
        }
        
        if (IsInHurt() || Ref.Star.IsInStar()) return false;
        
        Hurt();
        return true;
    }

    public void Hurt()
    {
        switch (Globalvar.Player.State)
        {
            case Globalvar.PlayerState.Small:
                Die();
                break;
            case Globalvar.PlayerState.Super:
                PowerDownTo(Globalvar.PlayerState.Small);
                break;
            default:
                PowerDownTo(Globalvar.PlayerState.Super);
                break;
        }
    }

    private void PowerDownTo(Globalvar.PlayerState state)
    {
        HurtTimer = HurtTime;
        Globalvar.Player.State = state;
        EmitSignal(SignalName.PowerDown);
    }

    public void Die()
    {
        // TODO: death animation and retry behaviors
        EmitSignal(SignalName.Dead);
    }
}