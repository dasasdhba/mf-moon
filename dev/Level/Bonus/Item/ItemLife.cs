using Component;
using Global;
using Godot;

namespace Level;

public partial class ItemLife : ItemRef
{
    [ExportCategory("ItemLife")]
    [Export]
    public int Life { get ;set; } = 1;
    
    [Export]
    public Vector2 LifeOffset { get ;set; }
    
    [Export]
    public PackedScene LifeScene { get ;set; }
    public AsyncLoader<LifeEffect> LifeLoader { get; set; }

    public override void _EnterTree()
    {
        LifeLoader = new(this, LifeScene);
    }

    public void CreateLife(int count)
    {
        if (count <= 0) return;
        
        var lifeNode = LifeLoader.Create();
        lifeNode.Value = count;
        lifeNode.Position = Body.Position + LifeOffset;
        Body.AddSibling(lifeNode);
    }

    public override void ItemGet(PlayerRef player)
    {
        CreateLife(Life);
        Body.QueueFree();
    }
}