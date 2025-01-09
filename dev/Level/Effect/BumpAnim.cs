using Godot;
using System;
using Utils;

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

    public override void _EnterTree()
    {
        Origin = Position;
        
        this.AddProcess(BumpProcess);
    }

    public void BumpProcess(double delta)
    {
        if (!Active) return;
        
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
