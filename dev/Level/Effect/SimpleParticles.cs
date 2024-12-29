using Godot;
using Godot.Collections;
using Utils;

namespace Level;

public partial class SimpleParticles : DrawParticles2D
{
    [ExportCategory("SimpleParticles")]
    [Export]
    public Array<Texture2D> Textures { get ;set; } = [];

    [Export]
    public Rect2 GenerateRect { get; set; } = new Rect2(-16f, -16f, 32f, 32f);
    
    [Export]
    public bool GlobalCoords { get; set; } = true;
    
    [ExportGroup("Alpha")]
    [Export]
    public float AlphaFromMin { get; set; } = 1f;
    
    [Export]
    public float AlphaFromMax { get; set; } = 1f;
    
    [Export]
    public float AlphaSpeed { get ;set; } = 3f;
    
    [ExportGroup("Scale")]
    [Export]
    public float ScaleFromMin { get; set; } = 1f;
    
    [Export]
    public float ScaleFromMax { get; set; } = 1f;
    
    [Export]
    public float ScaleTo { get; set; } = 0f;
    
    [Export]
    public float ScaleSpeed { get; set; } = 2f;
    
    [ExportGroup("Rotation")]
    [Export]
    public float RotationMin { get; set; } = 0f;
    
    [Export]
    public float RotationMax { get; set; } = 0f;
    
    [Export]
    public float RotationSpeed { get; set; } = 0f;

    protected override void ParticleSetup()
    {
        var texture = Textures[Mathe.RandiRange(0, Textures.Count - 1)];
        
        var pos = GenerateRect.Position + 
            new Vector2(Mathe.RandfRange(0f, GenerateRect.Size.X), Mathe.RandfRange(0f, GenerateRect.Size.Y));
    
        if (GlobalCoords) pos = GlobalTransform * pos;
    
        var alpha = Mathe.RandfRange(AlphaFromMin, AlphaFromMax);
        var scale = Mathe.RandfRange(ScaleFromMin, ScaleFromMax);
        var rotation = Mathe.RandfRange(RotationMin, RotationMax);
        
        AddDrawProcess(delta =>
        {
            alpha = Mathf.MoveToward(alpha, 0f, (float)(AlphaSpeed * delta));
            scale = Mathf.MoveToward(scale, ScaleTo, (float)(ScaleSpeed * delta));
            rotation += (float)(RotationSpeed * delta);

            if (GlobalCoords)
            {
                SetDrawGlobal(true);
                SetDrawPosition(pos);
            }
            else
            {
                SetDrawPosition(pos);
            }
            
            SetDrawScale(scale * Vector2.One);
            SetDrawRotation(Mathf.DegToRad(rotation));
            SetDrawModulateAlpha(alpha);
            
            QueuedDrawTexture(texture, Vector2.Zero);
            
            return alpha <= 0f;
        });
    }
}