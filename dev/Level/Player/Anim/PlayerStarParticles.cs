using Global;
using Godot;
using Utils;

namespace Level;

public partial class PlayerStarParticles : SimpleParticles
{
    [ExportCategory("PlayerStarParticles")]
    [Export]
    public Rect2 SmallRect { get; set; } = new(-13f, -15f, 28f, 29f);
    
    [Export]
    public Rect2 SuperRect { get ;set; } = new(-13f, -46f, 28f, 56f);
    
    [ExportGroup("MusicSettings")]
    [Export]
    public AudioStream StarMusic { get ;set; }
    
    [Export]
    public float MusicFadePoint { get ;set; } = 0.5f;
    
    [ExportGroup("Dependency")]
    [Export]
    public PlayerRef Ref { get; set; }
    
    private MusicSingleton.MusicPlayer StarMusicPlayer;

    public override void _EnterTree()
    {
        if (StarMusicPlayer != null) return;
    
        StarMusicPlayer = new()
        {
            Stream = StarMusic,
            FadeTime = Ref.Star.RunningOutTime * (1f - MusicFadePoint)
        };
        AddChild(StarMusicPlayer);
    }

    public override void _Ready()
    {
        Ref.Star.SignalStarted += StarMusicStart;
        Ref.Star.SignalEnded += StarMusicEnd;
        TreeExited += StarMusicEnd;
        
        Ref.Star.SignalRunningOut += () =>
        {
            var fadeTime = Ref.Star.RunningOutTime * MusicFadePoint;
            this.ActionDelay(fadeTime, () => StarMusicPlayer.FadeStop());
        };
    }

    private void StarMusicStart()
    {
        StarMusicPlayer.DirectPlay();
        Moon.Music.PauseAll();
    }

    private void StarMusicEnd()
    {
        StarMusicPlayer.Stop();
        Moon.Music.FadeResumeAll();
    }

    public override void _PhysicsProcess(double delta)
    {
        GenerateRect = Ref.Shape.IsSmall() ? SmallRect : SuperRect;
        Emitting = Ref.Star.IsInStar();
    }
}