using System;
using System.Collections.Generic;
using dev.dobon.ataraxia.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        
        spriteBatch.Draw(_sprites[sprite.TexturePath], transform.Position, Color.White);
    }
}