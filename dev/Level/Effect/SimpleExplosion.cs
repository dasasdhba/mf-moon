using Component;
using Godot;

namespace Level;

public partial class SimpleExplosion : Node
{
    [ExportCategory("SimpleExplosion")]
    [Export]
    public Node2D Root { get; set; }
    
    [Export]
    public Node2DCaster BoomCaster { get; set; }
    
    [Export]
    public AudioOneshotPlayer ExplosionSound { get; set; }

    [Signal]
    public delegate void ExplodedEventHandler();

    public void Explode()
    {
        BoomCaster?.Cast();
        ExplosionSound?.PlayOneshot();
        Root?.QueueFree();
        EmitSignal(SignalName.Exploded);
    }
}