using Global;
using Godot;

namespace Level;

public partial class LifeEffect : Node2D
{
    [ExportCategory("ScoreEffect")]
    [Export]
    public int Value { get ;set; } = 1;

    public override void _Ready()
    {
        Globalvar.Player.Life += Value;
    }
}