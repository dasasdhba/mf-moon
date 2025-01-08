using Godot;
using Utils;

namespace Level;

public partial class BlockCoin : Node
{
    [ExportCategory("BlockCoin")]
    [Export]
    public BlockItem BlockRef { get; set; }
    
    [Export]
    public double CoinTime { get ;set; } = 6d;
    
    [Signal]
    public delegate void FinishedEventHandler();
    
    private bool TimerStarted { get; set; } = false;
    private bool Timeout { get; set; } = false;

    public override void _EnterTree()
    {
        BlockRef.SignalCoinSpawned += (coin) =>
        {
            if (!TimerStarted && CoinTime > 0d)
            {
                TimerStarted = true;
                this.ActionDelay(CoinTime, () => Timeout = true);
            }

            if (Timeout)
            {
                BlockRef.Oneshot = true;
                EmitSignal(SignalName.Finished);
            }
        };
    }
    
#if TOOLS
    public override void _Ready()
    {
        if (BlockRef.DefaultLoader != null)
        {
            GD.PushWarning("Loading item in Coin Brick is not expected.");
        }
    }
#endif

}