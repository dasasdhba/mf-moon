using Godot;

namespace Level;

public partial class FireballExplosion : SimpleExplosion
{
    public override void _EnterTree()
    {
        if (Root is IPlatformer2D platformer)
        {
            platformer.SignalWallCollided += Explode;
            platformer.SignalCeilingCollided += Explode;
        }
    }
}