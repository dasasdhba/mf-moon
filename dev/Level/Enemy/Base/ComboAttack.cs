using Component;
using Godot;

namespace Level;

/// <summary>
/// Must own Combo1-7 AudioStreamPlayer Children.
/// </summary>
public partial class ComboAttack : Attacker
{
    [ExportCategory("ComboAttack")]
    [ExportGroup("ScoreSettings")]
    [Export]
    public Vector2 ScoreOffset { get ;set; }
    
    [Export]
    public PackedScene ScoreScene { get ;set; }
    public AsyncLoader<ScoreEffect> ScoreLoader { get; set; }
    
    [ExportGroup("LifeSettings")]
    [Export]
    public Vector2 LifeOffset { get ;set; }
    
    [Export]
    public PackedScene LifeScene { get ;set; }
    public AsyncLoader<LifeEffect> LifeLoader { get; set; }

    public override void _EnterTree()
    {
        ScoreLoader = new(this, ScoreScene);
        LifeLoader = new(this, LifeScene);
    }

    public void CreateLife(int count)
    {
        if (count <= 0) return;
        
        var lifeNode = LifeLoader.Create();
        lifeNode.Value = count;
        lifeNode.Position = Body.Position + LifeOffset;
        Body.AddSibling(lifeNode);
    }

    public void CreateScore(int score)
    {
        if (score <= 0) return;
        
        var scoreNode = ScoreLoader.Create();
        scoreNode.Value = score;
        scoreNode.Position = Body.Position + ScoreOffset;
        Body.AddSibling(scoreNode);
    }
    
    protected uint Combo { get ;set; } = 0;

    protected void AddCombo()
    {
        GetNode<AudioStreamPlayer>("Combo" + (Combo + 1)).Play();
        
        if (Combo < 6)
        {
            CreateScore(Combo switch
            {
                0 => 100,
                1 => 200,
                2 => 500,
                3 => 1000,
                4 => 2000,
                5 => 5000,
                _ => 100
            });
            Combo++;
        }
        else
        {
            CreateLife(1);
            Combo = 0;
        }
    }

    public ComboAttack() : base()
    {
        SignalAttackedValid += (enemy) => AddCombo();
    }
}