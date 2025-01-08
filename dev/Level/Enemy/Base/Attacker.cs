using Godot;
using Utils;

namespace Level;

/// <summary>
/// Attacker tries to attack enemy.
/// </summary>
public partial class Attacker : Node
{
    [ExportCategory("Attacker")]
    [Export]
    public CollisionObject2D Body { get ;set; }
    
    [Export]
    public EnemyAttacked.AttackType Type { get; set; }
    
    [Export]
    public bool Disabled { get; set; } = false;
    
    [Export(PropertyHint.Layers2DPhysics)]
    public uint AttackMask
    {
        get => _AttackMask;
        set
        {
            _AttackMask = value;
            if (Overlap != null) Overlap.CollisionMask = value;
        }
    }
    private uint _AttackMask = 1;
    
    /// <summary>
    /// Emit if the attack is not ignored.
    /// </summary>
    [Signal]
    public delegate void AttackedEventHandler(EnemyAttacked enemy, int respond);
    
    [Signal]
    public delegate void AttackedValidEventHandler(EnemyAttacked enemy);
    
    [Signal]
    public delegate void AttackedInvalidEventHandler(EnemyAttacked enemy);

    public virtual EnemyAttacked.RespondType Attack(EnemyAttacked enemy)
    {
        var res = enemy.TryCast(Type);
        if (res != EnemyAttacked.RespondType.Ignore)
        {
            EmitSignal(SignalName.Attacked, enemy, (int)res);
        }
        return res;
    }
    
    public void TryAttack()
    {
        foreach (var result in Overlap.GetOverlappingObjects(
                     r => EnemyAttacked.HasEnemyAttacked(r.Collider),
                     Vector2.Zero,
                     true
                 ))
        {
            var enemy = EnemyAttacked.GetEnemyAttacked(result.Collider);
            Attack(enemy);
        }
    }
    
    protected OverlapSync2D Overlap { get; set; }
    
    public Attacker() : base()
    {
        TreeEntered += () =>
        {
            if (Body != null)
            {
                Overlap = OverlapSync2D.CreateFrom(Body);
                Overlap.CollisionMask = AttackMask;
                
                this.AddPhysicsProcess(AttackProcess);
            }
        };
        
        SignalAttacked += (enemy, respond) =>
        {
            if (respond == (int)EnemyAttacked.RespondType.Valid)
            {
                EmitSignal(SignalName.AttackedValid, enemy);
            }
            else
            {
                EmitSignal(SignalName.AttackedInvalid, enemy);
            }
        };
    }

    protected virtual void AttackProcess(double delta)
    {
        if (Disabled) return;
        
        TryAttack();
    }
}