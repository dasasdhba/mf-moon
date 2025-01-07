using Global;
using Godot;
using GodotTask;
using Utils;

namespace Level;

public partial class PlayerShape : Node
{
    [ExportCategory("PlayerShape")]
    [Export]
    public float StuckPushSpeed { get; set; } = 100f;

    [Export] 
    public float StuckPushDownMargin { get; set; } = 32f;

    [ExportGroup("Dependency")] [Export] public PlayerRef Ref { get; set; }

    [Export] public CollisionShape2D SmallShape { get; set; }

    [Export] public CollisionShape2D SuperShape { get; set; }

    public bool IsStucked() => Stucked;
    private bool Stucked = false;
    
    public override void _EnterTree()
    {
        StuckDetect().Forget();
    }
    
    public bool IsSmall() => Globalvar.Player.State == Globalvar.PlayerState.Small
                             || Ref.Walk.IsCrouching();

    public override void _PhysicsProcess(double delta)
    {
        var isSmall = IsSmall();

        SmallShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, !isSmall);
        SuperShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, isSmall);
    }

    private bool IsOverlapping(Vector2 delta = default)
        => Ref.Body.IsOverlapping(delta);

    private bool TryPush(Vector2 dir)
        => Ref.Body.TryPushOut(StuckPushDownMargin * dir);

    private async GDTask StuckDetect()
    {
        while (true)
        {
            await Async.Delegate(this, () => !SuperShape.Disabled);

            Stucked = !Ref.MovementControl.IsDisabled() && IsOverlapping();
            if (Stucked)
            {
                Ref.MovementControl.Disable("Stuck");

                var gDir = -Ref.Body.UpDirection;
                var mDir = gDir.Orthogonal() * (Ref.Anim.FlipH ? -1 : 1);

                await Async.DelegatePhysicsProcess(this, (delta) =>
                {
                    if (TryPush(gDir)) return true;

                    Ref.Body.GlobalPosition -= mDir * (float)(StuckPushSpeed * delta);

                    return !IsOverlapping();
                });

                Stucked = false;
                Ref.ClearSpeed();
                Ref.MovementControl.Enable("Stuck");
            }

            await Async.Delegate(this, () => SuperShape.Disabled);
        }
    }
}