using Godot;

namespace Level;

/// <summary>
/// extend original enemy dead behaviors.
/// </summary>
public partial class EnemyDeadExtra : Node
{
    [ExportGroup("EnemyDeadExtra")]
    [Export]
    public EnemyAttacked.AttackType Type { get ;set; } = EnemyAttacked.AttackType.Stomp;

    protected EnemyDead Default { get; set; }

    public override void _EnterTree()
    {
        Default = GetParent<EnemyDead>();
    }
    
    public virtual bool IsExtraValid(EnemyAttacked.AttackType atk) => atk == Type;
    public virtual void CreateScore() => Default.CreateScore(Type);
    public virtual void Die() => Default.Die();
}