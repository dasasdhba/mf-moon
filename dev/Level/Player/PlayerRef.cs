using Global;
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
}