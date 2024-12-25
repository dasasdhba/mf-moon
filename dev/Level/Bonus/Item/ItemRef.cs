using Godot;
using Utils;

namespace Level;

/// <summary>
/// Actually, this can be used with any objects which need interact with player.
/// </summary>
public partial class ItemRef : Node
{
    private const string ItemRefTag = "ItemRef"; 
    
    public static bool HasItemRef(GodotObject node)
        => node.HasData(ItemRefTag);
    
    public static ItemRef GetItemRef(GodotObject node)
        => node.GetData<ItemRef>(ItemRefTag);

    [ExportCategory("ItemRef")]
    [Export]
    public CollisionObject2D Body
    {
        get => _Body;
        set
        {
            if (_Body != value)
            {
                _Body?.RemoveData(ItemRefTag);
                value?.SetData(ItemRefTag, this);
                _Body = value;
            }
        }
    }
    private CollisionObject2D _Body;
    
    [Export]
    public bool Disabled { get ;set; } = false;
    
    /// <summary>
    /// Called when the item is overlapping with player
    /// </summary>
    public virtual void ItemGet(PlayerRef player) { }
}