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
        
        var floorPositionX = MathF.Floor(transform.Position.X);
        var floorPositionY = MathF.Floor(transform.Position.Y);
        
        camera.Offset = new Vector2(-(transform.Position.X - floorPositionX), -(transform.Position.Y - floorPositionY));
        
        camera.Matrix = Matrix.CreateTranslation(-floorPositionX, -floorPositionY, 0.0f) *
                        Matrix.CreateTranslation(new Vector3(Game.LowResWidth * 0.5f, Game.LowResHeight * 0.5f, 0.0f));;
    }
}