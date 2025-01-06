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

        var positionX = transform.Position.X + Game.LowResWidth / 2;
        var positionY = transform.Position.Y + Game.LowResHeight / 2;
        
        camera.Matrix = Matrix.CreateTranslation(positionX, positionY, 0) * Matrix.CreateScale(new Vector3(camera.Zoom, camera.Zoom, 1));
    }
}