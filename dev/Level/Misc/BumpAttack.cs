using Godot;

namespace Level;

public partial class BumpAttack : Attacker
{
    public override EnemyAttacked.RespondType Attack(EnemyAttacked enemy)
    {
        if (enemy.Body is not IPlatformer2D platformer) 
            return EnemyAttacked.RespondType.Ignore;
        
        if (!platformer.IsOnFloor()) 
            return EnemyAttacked.RespondType.Ignore;
    
        return base.Attack(enemy);
    }
}