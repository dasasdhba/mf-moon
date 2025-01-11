using Godot;

namespace Level;

public partial class BeetAttack : Attacker
{
    [ExportCategory("BeetAttack")]
    [Export]
    public BeetMovement Movement { get ;set; }
    
    private bool IsValid() 
        => !Movement.IsInWatered() && !Movement.IsBounceFinished();

    public override void _Ready()
    {
        SignalAttacked += (e, res) => Movement.TryBounce();
    }

    public override void _PhysicsProcess(double delta)
    {
        Disabled = !IsValid();
    }
}