using Godot;
using System.Collections.Generic;
using Utils;

namespace Level;

/// <summary>
/// The respond to specific attack.
/// </summary>
public partial class EnemyAttacked : Node
{
    private const string EnemyAttackedTag = "EnemyAttacked"; 
    
    public static bool HasEnemyAttacked(GodotObject node)
        => node.HasData(EnemyAttackedTag);
    
    public static EnemyAttacked GetEnemyAttacked(GodotObject node)
        => node.GetData<EnemyAttacked>(EnemyAttackedTag);

    [ExportCategory("EnemyAttacked")]
    [Export]
    public CollisionObject2D Body
    {
        get => _Body;
        set
        {
            if (_Body != value)
            {
                _Body?.RemoveData(EnemyAttackedTag);
                value?.SetData(EnemyAttackedTag, this);
                _Body = value;
            }
        }
    }
    private CollisionObject2D _Body;
    
    [Export]
    public EnemyDead Dead { get ;set; }
    
    /// <summary>
    /// Enemy in disabled state will ignore everything.
    /// </summary>
    [Export]
    public bool Disabled { get; set; } = false;
    
    [Export]
    public int ShellHardness { get; set; } = 0;
    
    public enum RespondType { Valid, Invalid, Ignore }

    public enum AttackType
    {
        Misc,
        Stomp,
        Fireball,
        Beet,
        Shell,
        Star,
        Bump,
        Lava
    }
    
    protected Dictionary<AttackType, RespondType> Settings { get ;set; }
    
    // the exported dictionary in godot is awful currently
    // so we do some dirty work here
    
    [ExportGroup("InitSettings")]
    [Export]
    protected RespondType Stomp { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Fireball { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Beet { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Shell { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Star { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Bump { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Lava { get; set; } = RespondType.Valid;
    
    [Export]
    protected RespondType Misc { get; set; } = RespondType.Valid;

    public override void _EnterTree()
    {
        Settings = new()
        {
            { AttackType.Stomp, Stomp },
            { AttackType.Fireball, Fireball },
            { AttackType.Beet, Beet },
            { AttackType.Shell, Shell },
            { AttackType.Star, Star },
            { AttackType.Bump, Bump },
            { AttackType.Lava, Lava },
            { AttackType.Misc, Misc }
        };
    }
    
    public void SetSettings(AttackType atk, RespondType respond)
        => Settings[atk] = respond;
        
    public RespondType GetSettings(AttackType atk)
        => Settings[atk];
    
    /// <summary>
    /// return the respond.
    /// </summary>
    public RespondType TryCast(AttackType atk)
    {
        var res = GetSettings(atk);
        
        if (Disabled || res == RespondType.Ignore) return res;
        
        if (res == RespondType.Valid)
        {
            Dead.Cast(atk);
        }
        
        return res;
    }
}