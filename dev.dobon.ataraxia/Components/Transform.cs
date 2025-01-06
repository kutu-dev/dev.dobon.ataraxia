using dev.dobon.ataraxia.Interfaces;
using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia.Components;

public sealed class Transform(Vector2 position): IComponent, IDefault<Transform>
{
    public Vector2 Position = position;

    public static Transform Default()
    {
        return new Transform(Vector2.Zero);
    }
}