using Component;
using Global;
using Godot;

namespace Level;

public partial class BlockItem : BlockRef
{
    [ExportCategory("BlockItem")]
    [Export]
    public bool Oneshot { get; set; } = true;
    
    [Export]
    public bool AutoMushroom { get; set; } = true;
    
    [ExportGroup("Default")]
    [Export]
    public PackedScene DefaultItem { get; set; }
    public AsyncLoader<Node2D> DefaultLoader { get; set; }
    
    [Export]
    public Vector2 DefaultOffset { get; set; }
    
    [ExportGroup("Mushroom")]
    [Export]
    public PackedScene MushroomScene { get; set; }
    public AsyncLoader<Node2D> MushroomLoader { get; set; }
    
    [Export]
    public Vector2 MushroomOffset { get; set; } = new(0f, 0f);
    
    [ExportGroup("Coin")]
    [Export]
    public PackedScene CoinEffect { get; set; }
    public AsyncLoader<Node2D> CoinLoader { get; set; }
    
    [Export]
    public Vector2 CoinOffset { get; set; } = new(0f, -32f);

    [Signal]
    public delegate void CoinSpawnedEventHandler(Node2D coin);
    
    [Signal]
    public delegate void ItemSpawnedEventHandler(Node2D item);
    
    protected Node2D LoadedItem { get; set; }
    protected bool IsItemPowerUp { get; set; } = false;

    /// <summary>
    /// Load item which is already in tree, this would be called by ItemSprout.
    /// Failed if DefaultItem is not null.
    /// </summary>
    public bool TryLoadItem(Node2D item)
    {
        if (DefaultItem != null) return false;
        LoadedItem = item;
        LoadedItem.GetParent().CallDeferred(Node.MethodName.RemoveChild, LoadedItem);
        IsItemPowerUp = ItemRef.HasItemRef(item) && ItemRef.GetItemRef(item) is ItemPowerUp;
        
        return true;
    }

    public BlockItem() : base()
    {
        Ready += () =>
        {
            if (LoadedItem != null) DefaultLoader = new(this, LoadedItem.SceneFilePath);
            else if (DefaultItem != null) DefaultLoader = new(this, DefaultItem);
            
            if (DefaultLoader == null) CoinLoader = new(this, CoinEffect);
            else MushroomLoader = new(this, MushroomScene);
        };
        
        SignalHit += (hit, data) => Activate(data);
    }
    
    private bool ShouldCreateMushroom()
        => AutoMushroom && IsItemPowerUp && Globalvar.Player.State == Globalvar.PlayerState.Small;
    
    private Node2D CreateItem()
    {
        if (ShouldCreateMushroom())
        {
            var mushroom = MushroomLoader.Create();
            mushroom.Position = Body.Position + MushroomOffset;
            return mushroom;
        }
        
        Node2D item;
        if (LoadedItem != null)
        {
            item = LoadedItem;
            LoadedItem = null;
            
            item.Position += Body.Position;
        }
        else
        {
            item = DefaultLoader.Create();
            item.Position = Body.Position + DefaultOffset;
            
            IsItemPowerUp = ItemRef.HasItemRef(item) && ItemRef.GetItemRef(item) is ItemPowerUp;
            if (ShouldCreateMushroom())
            {
                item.QueueFree();
                var mushroom = MushroomLoader.Create();
                mushroom.Position = Body.Position + MushroomOffset;
                return mushroom;
            }
        }
        
        return item;
    }

    public void Activate(RefCounted data)
    {
        if (DefaultLoader != null)
        {
            var item = CreateItem();
            
            // TODO: make documentation
            var dir = data?.Get("SproutDirection");
            if (dir != null)
            {
                ItemSprout.GetItemSprout(item).Direction = (Vector2)dir;
            }
            
            EmitSignal(SignalName.ItemSpawned, item);
            Body.AddSibling(item);
        }
        else
        {
            var coin = CoinLoader.Create();
            coin.Position = Body.Position + CoinOffset;
            EmitSignal(SignalName.CoinSpawned, coin);
            Body.AddSibling(coin);
        }

        if (Oneshot)
        {
            Clear();
            QueueFree();
        }
    }
}