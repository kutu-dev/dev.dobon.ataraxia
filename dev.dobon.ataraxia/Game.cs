using System;
using System.Diagnostics;
using dev.dobon.ataraxia.Components;
using dev.dobon.ataraxia.Systems;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace dev.dobon.ataraxia;

public sealed partial class Game : Microsoft.Xna.Framework.Game
{
    private const int LowResWidth = 320;
    private const int LowResHeight = 180;
    
    private const int MinWindowWidth = LowResWidth * 3;
    private const int MinWindowHeight = LowResHeight * 3;
    
    private const int DefaultWindowWidth = 1280;
    private const int DefaultWindowHeight = 720;
    
    private GraphicsDeviceManager _graphics;
    private RenderTarget2D? _lowResRenderTarget;
    
    private Ecs _ecs = new();
    
    private SpriteBatch? _spriteBatch;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;
    
    Camera _camera = new();

    public Game()
    {
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
        
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = DefaultWindowWidth;
        _graphics.PreferredBackBufferHeight = DefaultWindowHeight;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = _loggerFactory.CreateLogger<Game>();
        
        _ecs.RegisterComponent<Transform>();
        _ecs.RegisterComponent<Kinematics>();
        _ecs.RegisterComponent<Sprite>();

        _ecs.RegisterSystem<Movement>();
        _ecs.RegisterSystem<RenderSprite>();

        var entity = _ecs.CreateEntity();
        _ecs.AddComponentToEntity<Transform>(entity);
        _ecs.AddComponentToEntity(entity, new Kinematics(new Vector2(10.0f, 0.0f), Vector2.Zero));
        _ecs.AddComponentToEntity(entity, new Sprite("Pear"));
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        // TODO: This doesn't affert the real window size
        // TODO: Check: https://github.com/MonoGame/MonoGame/issues/8621
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width < MinWindowWidth ? MinWindowWidth : Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height < MinWindowHeight ? MinWindowHeight : Window.ClientBounds.Height;
        
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        _lowResRenderTarget = new RenderTarget2D(GraphicsDevice, LowResWidth, LowResHeight);
        _camera.adjustPosition(0, 0);
        
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
        GraphicsDevice.SetRenderTarget(_lowResRenderTarget);
        GraphicsDevice.Clear(Color.Black);

        if (_spriteBatch is null)
        {
            SpriteBatchUnavailableCritical(_logger);
            Exit();
            return;
        }
        
        _spriteBatch.Begin(transformMatrix: _camera.getMatrix());
        _ecs.RenderSystems(gameTime, Content, _spriteBatch);
        _spriteBatch.End();
        
        RenderLowRes(_spriteBatch);
        
        base.Draw(gameTime);
    }
    
    [LoggerMessage(LogLevel.Critical, "The sprite batch has not been initialized")]
    public static partial void SpriteBatchUnavailableCritical(ILogger logger);
}