using Godot;

namespace Level;

public partial class PlayerGetItem : Node
{
    [ExportCategory("PlayerGetItem")]
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }

    protected OverlapSync2D Overlap { get ;set; }
    public override void _EnterTree()
    {
        Overlap = OverlapSync2D.CreateFrom(Ref.Body);
        Overlap.CollisionMask = 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Ref.InteractionControl.IsDisabled()) return;

        foreach (var result in Overlap.GetOverlappingObjects(
                     r => ItemRef.HasItemRef(r.Collider),
                     Vector2.Zero,
                     true
                 ))
        {
            var item = ItemRef.GetItemRef(result.Collider);
            if (item.Disabled) return;
            
            item.ItemGet(Ref);
        }
    }
}