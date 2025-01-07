using dev.dobon.ataraxia.Interfaces;
using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia.Components;

public class Camera(float zoom): IComponent, IDefault<Camera>
{
    public Matrix Matrix = Matrix.CreateTranslation(0,0,0);
    public Vector2 Offset = Vector2.Zero;
    
    public float Zoom = zoom;
    
    public static Camera Default()
    {
        return new Camera(1.0f);
    }
}