using Godot.Collections;

namespace Godot;

[GlobalClass, Tool]
public partial class ParallaxLayer2D : ParallaxLayer
{
    [ExportCategory("ParallaxLayer2D")]
    [Export]
    public Vector2 AutoScroll { get ;set; }

    public override void _ValidateProperty(Dictionary property)
    {
        // disable transform
        if (
            (string)property["name"] == "position" ||
            (string)property["name"] == "rotation" ||
            (string)property["name"] == "scale" ||
            (string)property["name"] == "skew"
        )
        {
            property["usage"] = (uint)PropertyUsageFlags.None;
        }
    }

    public override void _Process(double delta)
    {
#if TOOLS    
        if (Engine.IsEditorHint())
        {
            Transform = new(0f, Vector2.Zero);
            return;
        }
#endif        
        
        var offset = MotionOffset;

        if (MotionMirroring.X > 0f)
        {
            offset.X += (float)(AutoScroll.X * delta);
            offset.X = Mathf.Wrap(offset.X, 0f, MotionMirroring.X);
        }

        if (MotionMirroring.Y > 0f)
        {
            offset.Y += (float)(AutoScroll.Y * delta);
            offset.Y = Mathf.Wrap(offset.Y, 0f, MotionMirroring.Y);
        }
        
        MotionOffset = offset;
    }
}