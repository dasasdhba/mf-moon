using Component;
using Godot;
using Utils;

namespace Level;

// man, this is hard to parameterize.
// why should we remake this exactly?

public partial class FragmentCaster : Node
{
    [ExportCategory("FragmentCaster")]
    [Export]
    public Node2D Root { get ;set; }

    [Export]
    public PackedScene FragmentScene { get ;set; }
    
    public AsyncLoader<CharaPlatformer2D> FragmentLoader { get ;set; }

    public override void _EnterTree()
    {
        FragmentLoader = new(this, FragmentScene, 4);
    }

    public void Cast()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 offset;
            float gravity;
            float speed;

            switch (i)
            {
                case 0:
                    offset = new(5, 5);
                    gravity = -350f;
                    speed = 200f;
                    break;
                case 1:
                    offset = new(-6, 6);
                    gravity = -350f;
                    speed = -200f;
                    break;
                case 2:
                    offset = new(-8, -7);
                    gravity = -400f;
                    speed = -100f;
                    break;
                case 3:
                    offset = new(6, -6);
                    gravity = -400f;
                    speed = 100f;
                    break;
                default:
                    offset = new(0, 0);
                    gravity = 0.0f;
                    speed = 0.0f;
                    break;    
            }
            
            var fragment = FragmentLoader.Create();
            fragment.SetGravitySpeed(gravity);
            fragment.SetMoveSpeed(speed);
            fragment.Position = Root.Position + offset;
            if (speed < 0f) fragment.TryInitFlipAll();
            Root.AddSibling(fragment);
        }
    }
}