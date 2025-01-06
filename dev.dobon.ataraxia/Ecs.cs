using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using dev.dobon.ataraxia.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace dev.dobon.ataraxia;

public interface IComponent;
public interface IComponentPool;

public sealed class ComponentPool<T>: IComponentPool where T: IComponent
{
    private List<T> _pool = [];

    public void Add(Entity entity, T value)
    {
        _pool.Insert(entity.Id, value);
    }
    
    public T? Get(Entity entity)
    {
        return _pool.ElementAtOrDefault(entity.Id);
    }
}

public sealed class Entity(int id)
{
    public int Id = id;
}

public interface ISystem
{
    public void Process(Ecs ecs, Entity entity, GameTime gameTime) {}
    
    public void Render(Ecs ecs, Entity entity, GameTime gameTime, ContentManager contentManager, SpriteBatch spriteBatch) {}
}

public sealed class Ecs
{
    private int _nextEntityId;
    private readonly Dictionary<Type, IComponentPool> _components = new();
    
    private List<ISystem> _systems = [];

    public Entity CreateEntity()
    {
        return new Entity(_nextEntityId++);
    }
    
    public void RegisterComponent<T>() where T : IComponent
    {
        _components[typeof(T)] = new ComponentPool<T>();
    }

    public void RegisterSystem<T>() where T : ISystem, new()
    {
        _systems.Add(new T());
    }

    public void ProcessSystems(GameTime gameTime)
    {
        foreach (var entityId in Enumerable.Range( 0, _nextEntityId ))
        {
            foreach (var system in _systems)
            {
                system.Process(this, new Entity(entityId), gameTime);
            }
        }
    }
    
    public void RenderSystems(GameTime gameTime, ContentManager contentManager, SpriteBatch spriteBatch)
    {
        foreach (var entityId in Enumerable.Range( 0, _nextEntityId ))
        {
            foreach (var system in _systems)
            {
                system.Render(this, new Entity(entityId), gameTime, contentManager, spriteBatch);
            }
        }
    }

    public ComponentPool<T> GetComponentPool<T>() where T : IComponent
    {
        return (ComponentPool<T>)_components[typeof(T)];
    }
    
    public T? GetComponentOfEntity<T>(Entity entity) where T : IComponent
    {
        return GetComponentPool<T>().Get(entity);
    }

    public void AddComponentToEntity<T>(Entity entity) where T : IComponent, IDefault<T>
    {
        AddComponentToEntity(entity, T.Default());
    }

    public void AddComponentToEntity<T>(Entity entity, T value) where T : IComponent
    {
        GetComponentPool<T>().Add(entity, value);
    }
}