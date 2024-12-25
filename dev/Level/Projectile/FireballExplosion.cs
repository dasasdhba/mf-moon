using Component;
using Godot;

namespace Level;

public partial class FireballExplosion : Node
{
    [ExportCategory("FireballExplosion")]
    [Export]
    public Node2D Fireball { get; set; }
    
    [Export]
    public Node2DCaster Boom { get; set; }
    
    [Export]
    public AudioOneshotPlayer ExplosionSound { get; set; }

    [Signal]
    public delegate void ExplodedEventHandler();

    public override void _EnterTree()
    {
        if (Fireball is IPlatformer2D platformer)
        {
            platformer.SignalWallCollided += Explode;
            platformer.SignalCeilingCollided += Explode;
        }
    }

    public void Explode()
    {
        Boom?.Cast();
        ExplosionSound?.PlayOneshot();
        Fireball?.QueueFree();
        EmitSignal(SignalName.Exploded);
    }
}