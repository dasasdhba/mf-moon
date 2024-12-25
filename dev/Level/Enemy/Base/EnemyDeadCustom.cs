using Component;
using Godot;

namespace Level;

/// <summary>
/// Override enemy dead behavior with custom one.
/// </summary>
public partial class EnemyDeadCustom : EnemyDeadExtra
{
    [ExportCategory("EnemyDeadCustom")]
    [Export]
    public PackedScene DeadScene { get ;set; }
    public AsyncLoader<Node2D> DeadLoader { get ;set; }
    
    [Export]
    public Vector2 DeadOffset { get ;set; }

    public EnemyDeadCustom() : base()
    {
        TreeEntered += () =>
        {
            if (DeadScene != null)
            {
                DeadLoader = new(this, DeadScene);
            }    
        };
    }

    public override void Die()
    {
        var root = Default.Body;
        if (DeadLoader != null)
        {
            var dead = DeadLoader.Create();
            dead.Position = root.Position + DeadOffset;
            root.AddSibling(dead);
        }
        
        root.QueueFree();
    }
}