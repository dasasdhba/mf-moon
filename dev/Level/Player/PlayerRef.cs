using Component;
using Godot;

namespace Level;

/// <summary>
/// Keep dependency reference of the player node.
/// </summary>
public partial class PlayerRef : Node
{
    [ExportCategory("PlayerRef")]
    [ExportGroup("Refs")]
    [Export]
    public CharaPlatformer2D Body { get ;set; }
    
    [Export]
    public Inputer Input { get ;set; }
    
    [Export]
    public PlayerWalk Walk { get; set; }
    
    [Export]
    public PlayerJump Jump { get; set; }
    
    [Export]
    public PlayerAnim Anim { get; set; }
    
    [Export]
    public PlayerShape Shape { get ;set; }
    
    [Export]
    public PlayerHurt Hurt { get ;set; }
    
    [Export]
    public PlayerStar Star { get ;set; }
    
    // multiplayer is not planned
    // static instance access can be very convenient in this case

    /// <summary>
    /// Global access of player instance.
    /// </summary>
    public static PlayerRef Instance { get; private set; }

    public PlayerRef() : base()
    {
        Instance = this;
        
        InputControl = new()
        {
            OnDisabled = () => Input = null,
            OnEnabled = () => Input = RealInput
        };
        
        MovementControl = new()
        {
            OnDisabled = () => Body.AutoProcess = false,
            OnEnabled = () => Body.AutoProcess = true
        };
    }

    public override void _EnterTree()
    {
        RealInput = Input;
    }
    
    private Inputer RealInput;
    public DisableList InputControl { get; set; }
    public DisableList MovementControl { get ;set; }
    public DisableList InteractionControl { get ;set; } = new();
    
    public void ClearSpeed()
    {
        Body.Gravity = 0f;
        Body.MoveSpeed = 0f;
        Walk.Move.Speed = 0f;
    }
}