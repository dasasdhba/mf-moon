using System.Collections.Generic;
using Component;
using Godot;

namespace Level;

public partial class EnemyDead : Node
{
    [ExportCategory("EnemyDead")]
    [Export]
    public Node2D Body { get ;set; }
    
    /// <summary>
    /// If true, the Cast method has to be called manually.
    /// </summary>
    [Export]
    public bool Manual { get ;set; } = false;
    
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
        
        TreeEntered += () =>
        {
            ScoreLoader = new(this, ScoreScene);
        
            if (DeadScene != null)
                DeadLoader = new(this, DeadScene);

            if (!Manual && EnemyAttacked.HasEnemyAttacked(Body))
            {
                var atked = EnemyAttacked.GetEnemyAttacked(Body);
                atked.SignalAttacked += (int atk) => Cast((EnemyAttacked.AttackType)atk);
            }
        };
    }
    
    protected List<EnemyDeadExtra> Extras { get ;set; } = [];

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
        scoreNode.Position = Body.Position + ScoreOffset;
        Body.AddSibling(scoreNode);
    }

    /// <summary>
    /// Default death behavior.
    /// </summary>
    public virtual void Die()
    {
        if (DeadLoader != null)
        {
            var dead = DeadLoader.Create();
            dead.Position = Body.Position + DeadOffset;
            Body.AddSibling(dead);
        }
        
        Body.QueueFree();
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