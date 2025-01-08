using Global;
using Godot;

namespace Level;

public partial class CoinRef : ItemRef
{
    [ExportCategory("Coin")]
    [Export]
    public int Value { get ;set; } = 1;
    
    [Signal]
    public delegate void GottenEventHandler();

    public override void ItemGet(PlayerRef player)
    {
        Globalvar.Player.Coin += Value;
        EmitSignal(SignalName.Gotten);
        Body.QueueFree();
    }
}