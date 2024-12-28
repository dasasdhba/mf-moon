namespace Level;

public partial class ItemStar : ItemRef
{
    public override void ItemGet(PlayerRef player)
    {
        player.Star.Start();
        Body.QueueFree();
    }
}