using Godot;
using System;

namespace Level;

/// <summary>
/// hit block anim
/// </summary>
public partial class BumpAnim : Node2D
{
    [ExportCategory("BumpAnim")]
    [Export]
    public float Height { get; set; } = 4f;

    [Export]
    public Vector2 Direction { get; set; } = new(0, -1f);

    [Export]
    public float Omega { get; set; } = 1000f;

    protected float Angle { get; set; } = 0f;
    protected bool Active { get; set; } = false;

    public void Start()
    {
        Angle = 0f;
        Active = true;
    }

    protected Vector2 Origin { get; set; }

    public override void _Ready() => Origin = Position;

    public override void _Process(double delta)
    {
        if (Active)
        {
            Angle += (float)(Omega * delta);
            Position = Origin + Direction *
                Height * (float)Math.Sin(Mathf.DegToRad(Angle));
            if (Angle >= 180f)
            {
                Angle = 0f;
                Position = Origin;
                Active = false;
            }

        }
    }
}
