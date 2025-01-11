using System;
using Godot;
using Utils;

namespace Level;

public partial class BlockHitPlatformer : BlockHit
{
    [ExportCategory("BlockHitPlatformer")]
    [ExportGroup("Jump")]
    [Export]
    public bool EnableJump { get; set; } = true;

    [Export]
    public int JumpHardness { get; set; } = 0;

    [Export]
    public float JumpMargin { get; set; } = 4f;

    [Export]
    public float JumpHiddenMargin { get; set; } = 8f;

    [ExportGroup("Fall")]
    [Export]
    public bool EnableFall { get; set; } = true;

    [Export]
    public int FallHardness { get; set; } = 0;

    [Export]
    public float FallMargin { get; set; } = 4f;

    [Export]
    public float FallHiddenMargin { get; set; } = 8f;

    [ExportGroup("Move")]
    [Export]
    public bool EnableMove { get; set; } = true;

    [Export]
    public int MoveHardness { get; set; } = 0;

    [Export]
    public float MoveMargin { get; set; } = 4f;

    [Export]
    public float MoveHiddenMargin { get; set; } = 8f;
    
    protected virtual RefCounted GetJumpData() => null;
    protected virtual RefCounted GetFallData() => null;
    protected virtual RefCounted GetMoveData() => null;
    
    public BlockHitPlatformer() : base()
    {
        TreeEntered += () =>
        {
            this.AddPhysicsProcess(TryHitHidden);
        };
        
        Ready += () =>
        {
            if (Body is not IPlatformer2D platformer) return;

            platformer.SignalCeilingCollided += () =>
            {
                if (EnableJump)
                    TryHit(JumpHardness, JumpMargin * 
                            -platformer.GetGravityDirection(),
                        GetJumpData());
            };

            platformer.SignalFloorCollided += () =>
            {
                if (EnableFall)
                    TryHit(FallHardness, FallMargin * 
                            platformer.GetGravityDirection(), 
                        GetFallData());
            };

            platformer.SignalWallCollided += (dir) =>
            {
                if (EnableMove)
                    TryHit(MoveHardness, MoveMargin * 
                            dir * platformer.GetMoveDirection(),
                        GetMoveData());
            };
        };
    }

    private bool IsOverlappingWith(BlockRef block, Vector2 offset = default)
        => Overlap.IsOverlappingWith(block.Body, offset);

    private void PushFromBlock(BlockRef block, Vector2 motion)
    {
        if (Body is PhysicsBody2D pBody)
        {
            pBody.TryPushOut(motion);
            return;
        }
        
        // as the body should be IPlatformer, the following code is unreachable
        // we keep it here for future use (maybe)
        
        var dir = motion.Normalized();
        do
        {
            Body.GlobalPosition += dir;
        } 
        while (IsOverlappingWith(block));
    }

    public void TryHitHidden()
    {
        if (Disabled) return;
        
        var platformer = (IPlatformer2D)Body;
        
        var gSpeed = platformer.GetGravitySpeed();
        var gDir = platformer.GetGravityDirection();
        var mSign = Math.Sign(platformer.GetMoveSpeed());
        var mDir = mSign * platformer.GetMoveDirection();
        
        foreach (var result in Overlap.GetOverlappingObjects(
                     r => BlockRef.HasBlockRef(r.Collider),
                     Vector2.Zero,
                     true
                 ))
        {
            var block = BlockRef.GetBlockRef(result.Collider);
            if (!block.Hidden || block.Disabled) continue;

            if (EnableJump && gSpeed < 0f && !IsOverlappingWith(block, JumpHiddenMargin * gDir))
            {
                TryHitManual(block, JumpHardness, GetJumpData());
                PushFromBlock(block, JumpHiddenMargin * gDir);
            }

            if (EnableFall && gSpeed > 0f && !IsOverlappingWith(block, FallHiddenMargin * -gDir))
            {
                TryHitManual(block, FallHardness, GetFallData());
                PushFromBlock(block, FallHiddenMargin * -gDir);
            }

            if (EnableMove && mSign != 0 && !IsOverlappingWith(block, MoveHiddenMargin * -mDir))
            {
                TryHitManual(block, MoveHardness, GetMoveData());
                PushFromBlock(block, MoveHiddenMargin * -mDir);
            }
        }
    }
}