using System;
using dev.dobon.ataraxia.Components;
using Microsoft.Xna.Framework;

namespace dev.dobon.ataraxia.Systems;

public class Movement: ISystem
{
    public void Process(Ecs ecs, Entity entity, GameTime gameTime)
    {
        var transform = ecs.GetComponentOfEntity<Transform>(entity);
        if (transform == null)
        {
            return;
        }
        
        var kinematics = ecs.GetComponentOfEntity<Kinematics>(entity);
        if (kinematics == null)
        {
            return;
        }

        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        kinematics.Velocity += kinematics.Acceleration * seconds;
        transform.Position += kinematics.Velocity * seconds;
    }
}