using System.Collections.Generic;
using Godot;
using Utils;

namespace Level;

public partial class PlayerStomp : Node
{
    [ExportCategory("PlayerStomp")]
    [Export]
    public float StompSpeed { get; set; } = 450f;
    
    [Export]
    public float StompJumpSpeed { get ;set; } = 650f;

    [Export(PropertyHint.Layers2DPhysics)]
    public uint StompMask
    {
        get => _StompMask;
        set
        {
            _StompMask = value;
            if (Overlap != null) Overlap.CollisionMask = value;
        }
    }
    private uint _StompMask = 1;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get ;set; }
    
    [Signal]
    public delegate void StompedEventHandler();
    
    private const float OverlapDelay = 0.2f;
    protected List<EnemyRef> OverlapDelayList = [ ];
    
    protected OverlapSync2D Overlap { get ;set; }
    public override void _EnterTree()
    {
        Overlap = OverlapSync2D.CreateFrom(Ref.Body);
        Overlap.CollisionMask = StompMask;
        this.AddPhysicsProcess(StompProcess);
    }

    protected void StompProcess(double delta)
    {
        if (Ref.InteractionControl.IsDisabled()) return;
    
        foreach (var result in Overlap.GetOverlappingObjects(
                     r => EnemyRef.HasEnemyRef(r.Collider),
                     Vector2.Zero,
                     true
                 ))
        {
            var e = EnemyRef.GetEnemyRef(result.Collider);
            if (e.Disabled) continue;
            
            var star = Ref.Star.IsInStar();
            if (star && EnemyAttacked.HasEnemyAttacked(e.Body))
            {
                var atk = EnemyAttacked.GetEnemyAttacked(e.Body);
                if (atk.GetSettings(EnemyAttacked.AttackType.Star)
                    == EnemyAttacked.RespondType.Valid)
                {
                    continue;
                }
            }
            
            // test delay, this approach could make sure the list is safe
            
            // ATTENTION: stomping flytroopa will generate normal troopa
            // disable newly spawned enemies for a while is necessary
            // or can we make it inherit the delay params here?
            
            if (OverlapDelayList.Contains(e)) continue;
            OverlapDelayList.Add(e);
            var delay = e.ActionDelay(OverlapDelay, null);
            delay.TreeExited += () => OverlapDelayList.Remove(e);
            
            // stomp and hurt detect
            
            if (CanStomp(e))
            {
                StompJump(e.StompSpeed, e.StompJumpSpeed);
                e.EmitSignal(EnemyRef.SignalName.Stomped, Ref);
            }
            else
            {
                e.EmitSignal(EnemyRef.SignalName.PlayerHit, Ref);

                if (e.Hurt)
                {
                    if (Ref.Hurt.TryHurt(e))
                    {
                        e.EmitSignal(EnemyRef.SignalName.PlayerHurt, Ref);
                    }
                }
            }
        }
    }

    protected void StompJump(float stompSpeed = -1f, float stompJumpSpeed = -1f)
    {
        if (stompSpeed < 0f) stompSpeed = StompSpeed;
        if (stompJumpSpeed < 0f) stompJumpSpeed = StompJumpSpeed;
        
        var jump = Ref.Input.GetKey("Jump").Pressed;
        Ref.Body.SetGravitySpeed(jump ? -stompJumpSpeed : -stompSpeed);
        
        EmitSignal(SignalName.Stomped);
    }
    
    /// <summary>
    /// return true if should stomp
    /// </summary>
    protected bool CanStomp(EnemyRef e)
    {
        if (e.AllowStomp != EnemyRef.StompType.Active) return false;
        
        var dir = -Ref.Body.UpDirection;
        var playerMotion = Ref.Body.GetLastMotion().Dot(dir);
        var enemyMotion = e.GetLastMotion().Dot(dir);
        if (playerMotion < enemyMotion)
        {
            return false;
        }
        
        return !Overlap.IsOverlappingWith(e.Body, 
        -dir * (playerMotion - enemyMotion + e.StompHeight));
    }
}