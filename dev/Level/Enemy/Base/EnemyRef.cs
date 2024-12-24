using Component;
using Godot.Collections;
using Godot;
using Utils;

namespace Level;

/// <summary>
/// Make the enemy hurt player, or allow it to be stomped.
/// </summary>
[GlobalClass, Tool]
public partial class EnemyRef : Node
{
    private const string EnemyRefTag = "EnemyRef";
    
    public static bool HasEnemyRef(GodotObject node)
        => node.HasData(EnemyRefTag);
        
    public static EnemyRef GetEnemyRef(GodotObject node)
        => node.GetData<EnemyRef>(EnemyRefTag);

    [ExportCategory("EnemyRef")]
    [Export]
    public CollisionObject2D Body
    {
        get => _Body;
        set
        {
            if (_Body != value)
            {
                if (!Engine.IsEditorHint())
                {
                    _Body?.RemoveData(EnemyRefTag);
                    value?.SetData(EnemyRefTag, this);
                }
                
                _Body = value;
            }
        }
    }
    private CollisionObject2D _Body;
    
    /// <summary>
    /// The player will have no effect on disabled enemy.
    /// </summary>
    [Export]
    public bool Disabled { get; set; } = false;
    
    public enum StompType { Invalid = -1, Disabled = 0, Active = 1 }
    
    /// <summary>
    /// Both Invalid and Disabled will disallow stomping.
    /// If you need change this property in runtime, use Disabled.
    /// Otherwise use Invalid to have better performance.
    /// </summary>
    [ExportGroup("StompSettings")]
    [Export]
    public StompType AllowStomp
    {
        get => _AllowStomp;
        set
        {
            _AllowStomp = value;
            NotifyPropertyListChanged();
        }
    }
    private StompType _AllowStomp = StompType.Active;
    
    [Export(PropertyHint.Range, "0.001,32,or_greater")]
    public float StompHeight { get; set; } = 16f;
    
    [Export]
    public float StompSpeed { get ;set; } = -1f;
    
    [Export]
    public float StompJumpSpeed { get; set; } = -1f;

    [ExportGroup("HurtSettings")]
    [Export]
    public bool Hurt
    {
        get => _Hurt;
        set
        {
            _Hurt = value;
            NotifyPropertyListChanged();
        }
    }
    
    private bool _Hurt = true;
    
    [Export]
    public bool Critical
    {
        get => _Critical;
        set
        {
            _Critical = value;
            NotifyPropertyListChanged();
        }
    }
    
    private bool _Critical = false;
    
    [Export]
    public bool IgnorePlayerHurt { get; set; } = true;
    
    [Export]
    public bool IgnorePlayerStar { get; set; } = true;
    
    [Signal]
    public delegate void StompedEventHandler(PlayerRef player);
    
    [Signal]
    public delegate void PlayerHitEventHandler(PlayerRef player);
    
    [Signal]
    public delegate void PlayerHurtEventHandler(PlayerRef player);
    
    public override void _ValidateProperty(Dictionary property)
    {
        var allowStomp = AllowStomp != StompType.Invalid;
        var critical = Hurt && Critical;
        if (
            ((string)property["name"] == "StompHeight" && !allowStomp) ||
            ((string)property["name"] == "StompSpeed" && !allowStomp) ||
            ((string)property["name"] == "StompJumpSpeed" && !allowStomp) ||
            ((string)property["name"] == "Critical" && !Hurt) ||
            ((string)property["name"] == "IgnorePlayerHurt" && !critical) ||
            ((string)property["name"] == "IgnorePlayerStar" && !critical)
        )
        {
            property["usage"] = (uint)PropertyUsageFlags.ReadOnly;
        }
    }

    public EnemyRef() : base()
    {
#if TOOLS
        if (Engine.IsEditorHint()) return;
#endif
    
        TreeEntered += () =>
        {
            if (AllowStomp != StompType.Invalid)
            {
                Recorder = new() { Target = Body };
                AddChild(Recorder);
            }

            if (EnemyAttacked.HasEnemyAttacked(Body))
            {
                var atk = EnemyAttacked.GetEnemyAttacked(Body);
                SignalStomped += (p) =>
                {
                    atk.TryCast(EnemyAttacked.AttackType.Stomp);
                };
            }
        };
    }
    
    private MotionRecorder2D Recorder;
    public Vector2 GetLastMotion()
        => Recorder?.GetLastMotion() ?? Vector2.Zero;
    public Vector2 GetLastSpeed()
        => Recorder?.GetLastVelocity() ?? Vector2.Zero;
}

