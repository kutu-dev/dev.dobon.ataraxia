using System;
using System.Collections.Generic;
using dev.dobon.ataraxia.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace dev.dobon.ataraxia.Systems;

public sealed class RenderSprite: ISystem
{
    private Dictionary<string, Texture2D> _sprites = new Dictionary<string, Texture2D>();
    
    public void Render(Ecs ecs, Entity entity, GameTime gameTime, ContentManager contentManager, SpriteBatch spriteBatch)
    {
        var transform = ecs.GetComponentOfEntity<Transform>(entity);
        if (transform == null)
        {
            return;
        }
        
        var sprite = ecs.GetComponentOfEntity<Sprite>(entity);
        if (sprite == null)
        {
            return;
        }
        
        if (!_sprites.ContainsKey(sprite.TexturePath))
        {
            _sprites.Add(sprite.TexturePath, contentManager.Load<Texture2D>(sprite.TexturePath));
        }

        var spriteTexture = _sprites[sprite.TexturePath];

        // Just center the sprite until anchoring is needed 
        var spriteOrigin = new Vector2
        {
            X = spriteTexture.Width / 2,
            Y = spriteTexture.Height / 2
        };

        spriteBatch.Draw(spriteTexture, transform.Position, null, Color.White, transform.Rotation,
            spriteOrigin, transform.Scale, SpriteEffects.None, 0f);
    }
}