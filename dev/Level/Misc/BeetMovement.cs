namespace Level;

public partial class BeetMovement : BounceMovement
{
    private bool InWater;
    public bool IsInWatered() => InWater;

    public override void _PhysicsProcess(double delta)
    {
        if (!Body.IsInWater() || InWater) return;

        InWater = true;
        CancelGetThrough();
        Body.CollisionMask = 0;
        Body.SetMoveSpeed(0f);
        Body.SetGravitySpeed(150f);
    }
}