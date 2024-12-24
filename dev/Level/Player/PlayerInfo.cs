using Godot;

namespace Level;

/// <summary>
/// Provide some player info for enemy logic.
/// </summary>
public partial class PlayerInfo : Node
{
    public static bool IsPlayerValid() => IsInstanceValid(PlayerRef.Instance);
    public static Vector2 PlayerPosition { get; private set; }
    public static Vector2 PlayerGlobalPosition { get; private set; }
    public static bool PlayerFlipH { get; private set; }
    
    private void Update()
    {
        var player = PlayerRef.Instance;
        if (!IsInstanceValid(player)) return;
        
        PlayerPosition = player.Body.Position;
        PlayerGlobalPosition = player.Body.GlobalPosition;
        PlayerFlipH = player.Anim.FlipH;
    }
    
    public override void _EnterTree() => Update();
    public override void _PhysicsProcess(double delta) => Update();
}