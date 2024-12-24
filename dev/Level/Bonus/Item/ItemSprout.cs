using Godot;
using GodotTask;
using Utils;

namespace Level;

public partial class ItemSprout : Node
{
    private const string ItemSproutTag = "ItemSprout"; 
    
    public static bool HasItemSprout(GodotObject node)
        => node.HasData(ItemSproutTag);
    
    public static ItemSprout GetItemSprout(GodotObject node)
        => node.GetData<ItemSprout>(ItemSproutTag);

    [ExportCategory("ItemSprout")]
    [Export]
    public PhysicsBody2D Body
    {
        get => _Body;
        set
        {
            if (_Body != value)
            {
                _Body?.RemoveData(ItemSproutTag);
                value?.SetData(ItemSproutTag, this);
                _Body = value;
            }
        }
    }
    private PhysicsBody2D _Body;
    
    [Export]
    public float Speed { get ;set; } = 50f;
    
    [Export]
    public Vector2 Direction { get ;set; } = Vector2.Up;
    
    [Export]
    public int ZIndex { get ;set; } = -15;

    protected virtual void DisableBody()
    {
        if (Body is CharaPlatformer2D chara) chara.AutoProcess = false;
    }

    protected virtual void RestoreBody()
    {
        if (Body is CharaPlatformer2D chara) chara.AutoProcess = true;
    }
    
    protected virtual bool ShouldSprout() => Body.IsOverlapping();

    protected virtual bool SproutProcess(double delta)
    {
        var result = Body.IsOverlapping();
        if (result) Body.GlobalPosition += Direction * (float)(Speed * delta);
        return !result;
    }

    public async GDTaskVoid Sprout()
    {
        if (ItemRef.HasItemRef(Body))
            ItemRef.GetItemRef(Body).Disabled = true;
        var zOrigin = Body.ZIndex;
        Body.ZIndex = ZIndex;
        DisableBody();
        
        await GDTask.WaitForPhysicsProcess();
        await Async.DelegatePhysicsProcess(this, SproutProcess);
        
        if (ItemRef.HasItemRef(Body))
            ItemRef.GetItemRef(Body).Disabled = false;
        Body.ZIndex = zOrigin;
        RestoreBody();
    }
    
    public override void _EnterTree()
    {
        Sprout().Forget();
    }
}