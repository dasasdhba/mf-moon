using Component;
using Global;
using Godot;

namespace Level;

public partial class ItemPowerUp : ItemRef
{
    [ExportCategory("ItemPowerUp")]
    [Export]
    public Globalvar.PlayerState State { get ;set; } = Globalvar.PlayerState.Super;
    
    [Export]
    public bool Force { get ;set; } = false;
    
    [ExportGroup("ScoreSettings")]
    [Export]
    public int Score { get ;set; } = 100;
    
    [Export]
    public Vector2 ScoreOffset { get ;set; }
    
    [Export]
    public PackedScene ScoreScene { get ;set; }
    public AsyncLoader<ScoreEffect> ScoreLoader { get; set; }
    
    [Signal]
    public delegate void ReservedEventHandler();

    public override void _EnterTree()
    {
        ScoreLoader = new(this, ScoreScene);
    }

    public void CreateScore(int score)
    {
        if (score <= 0) return;
        
        var scoreNode = ScoreLoader.Create();
        scoreNode.Value = score;
        scoreNode.Position = Body.Position + ScoreOffset;
        Body.AddSibling(scoreNode);
    }

    public override void ItemGet(PlayerRef player)
    {
        CreateScore(Score);
        Body.QueueFree();
        
        if (Force)
        {
            if (Globalvar.Player.State != State)
            {
                Globalvar.Player.State = State;
            }
            else
            {
                EmitSignal(SignalName.Reserved);
            }
            return;
        }
        
        var reserved = State == Globalvar.PlayerState.Small || 
            State == Globalvar.Player.State ||
            (State == Globalvar.PlayerState.Super && (int)Globalvar.Player.State > 1);
        if (!reserved)
        {
            Globalvar.Player.State = Globalvar.Player.State == Globalvar.PlayerState.Small ? 
                Globalvar.PlayerState.Super : State;
        }
        else
        {
            EmitSignal(SignalName.Reserved);
        }
    }
}