using System.Collections.Generic;
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
    
    [Export]
    public PlayerStomp Stomp { get; set; }
    
    [Export]
    public PlayerShape Shape { get ;set; }

    public void ClearSpeed()
    {
        Body.Gravity = 0f;
        Body.MoveSpeed = 0f;
        Walk.Move.Speed = 0f;
    }

    public override void _EnterTree()
    {
        RealInput = Input;
    }

    private List<string> InputIgnoreList = [];
    private Inputer RealInput;
    
    public void IgnoreInput(string reason)
    {
        if (InputIgnoreList.Count == 0)
        {
            Input = null;
        }
        
        InputIgnoreList.Add(reason);
    }

    public void ResumeInput(string reason)
    {
        InputIgnoreList.Remove(reason);
        if (InputIgnoreList.Count == 0)
        {
            Input = RealInput;
        }
    }
    
    public bool IsInputIgnored() => InputIgnoreList.Count > 0;
    
    private List<string> DisableMovementList = [];
    public void DisableMovement(string reason)
    {
        if (DisableMovementList.Count == 0)
        {
            Body.AutoProcess = false;
        }
        DisableMovementList.Add(reason);
    }

    public void EnableMovement(string reason)
    {
        DisableMovementList.Remove(reason);
        if (DisableMovementList.Count == 0)
        {
            Body.AutoProcess = true;
        }
    }
    
    public bool IsMovementDisabled() => DisableMovementList.Count > 0;
    
    private List<string> DisableStompList = [];
    public void DisableStomp(string reason)
        => DisableStompList.Add(reason);
        
    public void EnableStomp(string reason)
        => DisableStompList.Remove(reason);
        
    public bool IsStompDisabled() => DisableStompList.Count > 0;
}