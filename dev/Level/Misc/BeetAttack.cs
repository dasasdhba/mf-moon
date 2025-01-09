using Godot;

namespace Level;

public partial class BeetAttack : Attacker
{
    [ExportCategory("BeetAttack")]
    [Export]
    public BeetMovement Movement { get ;set; }
    
    private bool Water;
    private bool IsInWater()
    {
        if (Water) return true;
        
        Water = Movement.Body.IsInWater();
        return Water;
    }
    
    private bool IsValid() => !IsInWater() && !Movement.IsBounceFinished();

    public override void _Ready()
    {
        SignalAttacked += (e, res) => Movement.TryBounce();
    }

    public override void _PhysicsProcess(double delta)
    {
        Disabled = !IsValid();
    }
}