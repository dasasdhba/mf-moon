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
    
    /// <summary>
    /// Emit if the attack is not ignored.
    /// </summary>
    [Signal]
    public delegate void AttackedEventHandler(EnemyAttacked enemy, int respond);

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
                Overlap.CollisionMask = 1;
                
                this.AddPhysicsProcess(AttackProcess);
            }
        };
    }

    protected virtual void AttackProcess(double delta)
    {
        if (Disabled) return;
        
        TryAttack();
    }
}