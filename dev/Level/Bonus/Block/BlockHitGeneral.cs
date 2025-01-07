using Godot;
using Utils;

namespace Level;

public partial class BlockHitGeneral : BlockHit
{
    [ExportCategory("BlockHitGeneral")]
    [Export]
    public int Hardness { get; set; } = 0;
    
    protected virtual RefCounted GetHitData() => null;

    public BlockHitGeneral() : base()
    {
        TreeEntered += () =>
        {
            this.AddPhysicsProcess(() => TryHit(Hardness, Vector2.Zero, GetHitData()));
        };
    }
}