using Component;
using Godot;

namespace Level;

public partial class PlayerStar : ComboAttack
{
    [ExportCategory("PlayerStar")]
    [Export]
    public double StarTime = 10d;
    
    [Export]
    public double RunningOutTime = 1d;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }
    
    [Signal]
    public delegate void StartedEventHandler();
    
    [Signal]
    public delegate void EndedEventHandler();
    
    [Signal]
    public delegate void RunningOutEventHandler();
    
    private double StarTimer = 0d;

    public void Start()
    {
        StarTimer = StarTime;
        EmitSignal(SignalName.Started);
    }
    public bool IsInStar() => StarTimer > 0d;

    protected override void AttackProcess(double delta)
    {
        if (Ref.InteractionControl.IsDisabled()) return;
    
        if (StarTimer > 0d)
        {
            var lastTime = StarTimer;
            StarTimer -= delta;
            if (lastTime >= RunningOutTime && StarTimer < RunningOutTime)
            {
                EmitSignal(SignalName.RunningOut);
            }

            if (StarTimer <= 0d)
            {
                Combo = 0;
                EmitSignal(SignalName.Ended);
            }
        }
        
        if (!IsInStar()) return;
        
        TryAttack();
    }
}