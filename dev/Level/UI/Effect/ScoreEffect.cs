using Global;
using Godot;

namespace Level;

public partial class ScoreEffect : Node2D
{
    [ExportCategory("ScoreEffect")]
    [Export]
    public int Value { get ;set; } = 100;

    public override void _Ready()
    {
        Globalvar.Player.Score += Value;
    }
}