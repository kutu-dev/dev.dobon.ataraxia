using System;
using dev.dobon.ataraxia.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace dev.dobon.ataraxia.Systems;

public class CalculateCameraMatrix: ISystem
{
    public void Render(Ecs ecs, Entity entity, GameTime gameTime, ContentManager contentManager, SpriteBatch spriteBatch)
    {
        var camera = ecs.GetComponentOfEntity<Camera>(entity);
        if (camera == null)
        {
            return;
        }
        
        var transform = ecs.GetComponentOfEntity<Transform>(entity);
        if (transform == null)
        {
            return;
        }
        
        var positionX = transform.Position.X;
        var positionY = transform.Position.Y;
        
        var floorPositionX = float.Floor(positionX);
        var floorPositionY = float.Floor(positionY);
        
        camera.Offset = new Vector2(floorPositionX - positionX, floorPositionY - positionY);
        
        camera.Matrix = Matrix.Invert(Matrix.CreateTranslation(floorPositionX, floorPositionY, 0));
    }
}