﻿using System;
using System.Diagnostics;
using dev.dobon.ataraxia.Components;
using dev.dobon.ataraxia.Systems;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace dev.dobon.ataraxia;

public sealed partial class Game : Microsoft.Xna.Framework.Game
{
    public const int LowResWidth = 320;
    public const int LowResHeight = 180;
    
    private const int MinWindowWidth = LowResWidth * 3;
    private const int MinWindowHeight = LowResHeight * 3;
    
    private const int DefaultWindowWidth = 1280;
    private const int DefaultWindowHeight = 720;
 
    private readonly ILogger _logger;
    
    private readonly GraphicsDeviceManager _graphics;
    private RenderTarget2D? _lowResRenderTarget;
    private SpriteBatch? _spriteBatch;

    private readonly Ecs _ecs = new();
    private Entity _activeCamera;

    public Game()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<Game>();
        
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
        
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = DefaultWindowWidth;
        _graphics.PreferredBackBufferHeight = DefaultWindowHeight;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        
        SetupEcs();
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        // TODO: This doesn't affect the real window size
        // TODO: Check: https://github.com/MonoGame/MonoGame/issues/8621
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width < MinWindowWidth ? MinWindowWidth : Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height < MinWindowHeight ? MinWindowHeight : Window.ClientBounds.Height;
        
        _graphics.ApplyChanges();
    }

    private void SetupEcs()
    {
        _ecs.RegisterComponent<Transform>();
        _ecs.RegisterComponent<Kinematics>();
        _ecs.RegisterComponent<Sprite>();
        _ecs.RegisterComponent<Camera>();

        _ecs.RegisterSystem<Movement>();
        _ecs.RegisterSystem<RenderSprite>();
        _ecs.RegisterSystem<CalculateCameraMatrix>();

        _activeCamera = _ecs.CreateEntity();
        _ecs.AddComponentToEntity<Transform>(_activeCamera);
        //_ecs.AddComponentToEntity(_activeCamera, new Kinematics(new Vector2(1f, 1f), Vector2.Zero));
        _ecs.AddComponentToEntity<Camera>(_activeCamera);
        
        var entity = _ecs.CreateEntity();
        _ecs.AddComponentToEntity(entity, new Transform(new Vector2(0.0f, 0.0f), 45.0f, new Vector2(2.5f, 2.0f)));
        _ecs.AddComponentToEntity(entity, new Sprite("Pear"));
    }

    protected override void Initialize()
    {
        _lowResRenderTarget = new RenderTarget2D(GraphicsDevice, LowResWidth, LowResHeight);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        _ecs.ProcessSystems(gameTime);
        
        base.Update(gameTime);
    }

    private void RenderLowRes(SpriteBatch spriteBatch)
    {
        GraphicsDevice.SetRenderTarget(null);
        
        var widthScale = _graphics.PreferredBackBufferWidth / (float)LowResWidth;
        var heightScale = _graphics.PreferredBackBufferHeight / (float)LowResHeight;
        
        var targetScale = Math.Min(widthScale, heightScale);
        
        var targetWidth = (int)(LowResWidth * targetScale);
        var targetHeight = (int)(LowResHeight * targetScale);
        
        var lowResWidthPosition = (_graphics.PreferredBackBufferWidth - targetWidth) / 2;
        var lowResHeightPosition = (_graphics.PreferredBackBufferHeight - targetHeight) / 2;

        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        spriteBatch.Draw(_lowResRenderTarget,
            new Rectangle(
                lowResWidthPosition,
                lowResHeightPosition,
                targetWidth,
                targetHeight),
            Color.White);
        spriteBatch.End();
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        GraphicsDevice.SetRenderTarget(_lowResRenderTarget);

        if (_spriteBatch is null)
        {
            SpriteBatchUnavailableCritical(_logger);
            Exit();
            return;
        }

        var transformMatrix = _ecs.GetComponentOfEntity<Camera>(_activeCamera)?.Matrix;
        if (transformMatrix is null)
        {
            ActiveCameraInvalidEntityCritical(_logger);
            Exit();
            return;
        }
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
        _ecs.RenderSystems(gameTime, Content, _spriteBatch);
        _spriteBatch.End();
        
        RenderLowRes(_spriteBatch);
        base.Draw(gameTime);
    }
    
    [LoggerMessage(LogLevel.Critical, "The sprite batch has not been initialized")]
    public static partial void SpriteBatchUnavailableCritical(ILogger logger);
    
    [LoggerMessage(LogLevel.Critical, "The active camera is not a valid entity" +
                                      "or the entity doesn't have a camera component")]
    public static partial void ActiveCameraInvalidEntityCritical(ILogger logger);
}