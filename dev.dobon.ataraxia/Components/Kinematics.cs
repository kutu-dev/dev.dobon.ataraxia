using dev.dobon.ataraxia.Interfaces;
using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia.Components;

public sealed class Kinematics(Vector2 velocity, Vector2 acceleration): IComponent, IDefault<Kinematics>
{
    public Vector2 Velocity = velocity;
    public Vector2 Acceleration = acceleration;

    public static Kinematics Default()
    {
        return new Kinematics(Vector2.Zero, Vector2.Zero);
    }
}