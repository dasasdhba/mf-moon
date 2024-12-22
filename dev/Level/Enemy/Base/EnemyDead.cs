using System.Collections.Generic;
using Component;
using Godot;

namespace Level;

public partial class EnemyDead : Node
{
    [ExportCategory("EnemyDead")]
    [Export]
    public Node2D Root { get ;set; }
    
    [ExportGroup("ScoreSettings")]
    [Export]
    public int Score { get ;set; } = 100;
    
    [Export]
    public Vector2 ScoreOffset { get ;set; }
    
    [Export]
    public PackedScene ScoreScene { get ;set; }
    public AsyncLoader<ScoreEffect> ScoreLoader { get ;set; }
    
    [ExportGroup("DeadSettings")]
    [Export]
    public PackedScene DeadScene { get ;set; }
    public AsyncLoader<Node2D> DeadLoader { get ;set; }
    
    [Export]
    public Vector2 DeadOffset { get ;set; }

    public EnemyDead() : base()
    {
        ChildEnteredTree += child =>
        {
            if (child is EnemyDeadExtra extra) 
                Extras.Add(extra);
        };
    }
    
    protected List<EnemyDeadExtra> Extras { get ;set; } = [];

    public override void _EnterTree()
    {
        ScoreLoader = new(this, ScoreScene);
        
        if (DeadScene != null)
            DeadLoader = new(this, DeadScene);
    }

    protected virtual int GetScore(EnemyAttacked.AttackType atk)
    {
        return atk is EnemyAttacked.AttackType.Shell or 
            EnemyAttacked.AttackType.Star or 
            EnemyAttacked.AttackType.Lava ? 
            0 : Score;
    }

    public void CreateScore(EnemyAttacked.AttackType atk)
        => CreateScore(GetScore(atk));

    public void CreateScore(int score)
    {
        if (score <= 0) return;
        
        var scoreNode = ScoreLoader.Create();
        scoreNode.Value = score;
        scoreNode.Position = Root.Position + ScoreOffset;
        Root.AddSibling(scoreNode);
    }

    /// <summary>
    /// Default death behavior.
    /// </summary>
    public virtual void Die()
    {
        if (DeadLoader != null)
        {
            var dead = DeadLoader.Create();
            dead.Position = Root.Position + DeadOffset;
            Root.AddSibling(dead);
        }
        
        Root.QueueFree();
    }
    
    public void Cast(EnemyAttacked.AttackType atk)
    {
        foreach (var extra in Extras)
        {
            if (extra.IsExtraValid(atk))
            {
                extra.CreateScore();
                extra.Die();
                return;
            }
        }
        
        CreateScore(atk);
        Die();
    }
}