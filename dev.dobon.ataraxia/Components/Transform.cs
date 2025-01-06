using dev.dobon.ataraxia.Interfaces;
using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia.Components;

public sealed class Transform(Vector2 position, float rotation, Vector2 scale): IComponent, IDefault<Transform>
{
    public Vector2 Position = position;
    public float Rotation = rotation;
    public Vector2 Scale = scale;

    public static Transform Default()
    {
        return new Transform(Vector2.Zero, 0.0f, Vector2.One);
    }
}