using Godot;
using Utils;

namespace Level;

/// <summary>
/// Block activated by BlockHit
/// </summary>
public partial class BlockRef : Node
{
    private const string BlockRefTag = "BlockRef"; 
    
    public static bool HasBlockRef(GodotObject node)
        => node.HasData(BlockRefTag);
    
    public static BlockRef GetBlockRef(GodotObject node)
        => node.GetData<BlockRef>(BlockRefTag);

    [ExportCategory("BlockRef")]
    [Export]
    public CollisionObject2D Body
    {
        get => _Body;
        set
        {
            if (_Body != value)
            {
                _Body?.RemoveData(BlockRefTag);
                value?.SetData(BlockRefTag, this);
                _Body = value;
            }
        }
    }
    private CollisionObject2D _Body;
    
    [Export]
    public bool Disabled { get ;set; } = false;
    
    [Export]
    public int Hardness { get ;set; } = -1;
    
    [ExportGroup("HiddenSettings")]
    [Export]
    public bool HideAtStart { get ;set; } = false;
    
    [Export(PropertyHint.Layers2DPhysics)]
    public uint HiddenLayer { get ;set; } = 1;
    
    public bool Hidden { get ;set; } = false;
    
    [Signal]
    public delegate void HitEventHandler(BlockHit hit, RefCounted data);
    
    [Signal]
    public delegate void HitFailedEventHandler(BlockHit hit, RefCounted data);
    
    protected uint OriginLayer { get; set; }
    public virtual void Hide()
    {
        if (Hidden) return;
        
        Hidden = true;
        OriginLayer = Body.CollisionLayer;
        Body.CollisionLayer = HiddenLayer;
        Body.Hide();
    }

    public virtual void Show()
    {
        if (!Hidden) return;
    
        Hidden = false;
        Body.CollisionLayer = OriginLayer;
        Body.Show();
    }

    public bool TryHit(BlockHit hit, int hardness, RefCounted data = null)
    {
        if (Disabled) return false;
        if (Hardness <= hardness) BlockHit(hit, data);
        else BlockHitFailed(hit, data);
        
        return true;
    }

    public void Clear()
    {
        Disabled = true;
        Body = null;
    }

    protected void BlockHit(BlockHit hit, RefCounted data = null)
    {
        Show();
        EmitSignal(SignalName.Hit, hit, data);
    }

    protected void BlockHitFailed(BlockHit hit, RefCounted data = null)
    {
        Show();
        EmitSignal(SignalName.HitFailed, hit, data);
    }

    public BlockRef() : base()
    {
        TreeEntered += () =>
        {
            if (HideAtStart) Hide();
        };
    }
}