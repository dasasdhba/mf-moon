using System.Collections.Generic;
using Godot;

namespace Level;

public partial class BlockHit : Node
{
    [ExportCategory("BlockHit")]
    [Export]
    public CollisionObject2D Body { get ;set; }
    
    [Export]
    public bool Disabled { get ;set; } = false;
    
    [Export(PropertyHint.Layers2DPhysics)]
    public uint HitMask
    {
        get => _HitMask;
        set
        {
            _HitMask = value;
            if (Overlap != null) Overlap.CollisionMask = value;
        }
    }
    private uint _HitMask = 1;

    protected OverlapSync2D Overlap { get ;set; }

    public BlockHit() : base()
    {
        TreeEntered += () =>
        {
            Overlap = OverlapSync2D.CreateFrom(Body);
            Overlap.CollisionMask = HitMask;
        };
    }

    public void TryHit(int hardness, Vector2 offset = default, RefCounted data = null)
    {
        if (Disabled) return;
    
        foreach (var result in Overlap.GetOverlappingObjects(
                     r => BlockRef.HasBlockRef(r.Collider),
                     offset,
                     true
                 ))
        {
            var block = BlockRef.GetBlockRef(result.Collider);

            if (ManualBlocks.Remove(block))
            {
                continue;
            }
            
            block.TryHit(this, hardness, data);
        }
    }
    
    // Manual hit block will not be hit by general hit detection once.
    // This prevent multiple hit for hidden blocks.
    
    private List<BlockRef> ManualBlocks = [];
    public void TryHitManual(BlockRef block, int hardness, RefCounted data = null)
    {
        if (Disabled || ManualBlocks.Contains(block)) return;
        
        ManualBlocks.Add(block);
        block.TryHit(this, hardness, data);
    }
}