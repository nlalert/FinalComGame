using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class StageTransitionScene : Scene
{
    private TextUI _stageName;
    private Rectangle _fullScreenRect;
    
    // Transition state tracking
    private enum TransitionState
    {
        SetUpText,
        FadingIn,
        ShowingMessage
    }
    
    private TransitionState _currentState;
    private float _opacity;
    private float _fadeInTime;  // Time to fade to black
    private float _messageTime; // Time to show the message
    private float _timer;

    public override void Initialize(GameManager gameManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(gameManager, graphicsDevice, graphicsDeviceManager, content);

        _fullScreenRect = new Rectangle(0, 0, Singleton.SCREEN_WIDTH, Singleton.SCREEN_HEIGHT);
        ResetTransition();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _timer += deltaTime;
        
        switch (_currentState)
        {
            case TransitionState.SetUpText:
                SetupHUD();
                _currentState = TransitionState.FadingIn;
                break;
            case TransitionState.FadingIn:
                // Fade to black
                _opacity = Math.Min(1f, _timer / _fadeInTime);
                
                if (_timer >= _fadeInTime)
                {
                    // Move to showing message phase
                    _currentState = TransitionState.ShowingMessage;
                    _timer = 0f;
                    _opacity = 1f; // Ensure we're fully opaque
                }
                break;
                
            case TransitionState.ShowingMessage:
                // Keep the screen black and show the message
                if (_timer >= _messageTime)
                {
                    // Prepare for next stage and transition
                    PrepareNextStage();
                }
                break;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        _spriteBatch.Draw(Singleton.Instance.PixelTexture, _fullScreenRect, Color.Black * _opacity);
        _spriteBatch.End();
    }
    
    protected override void SetupHUD()
    {
        _ui.ClearHUD();

        int textWidth = Singleton.SCREEN_WIDTH * 3 / 4;
        int textHeight = 80;
        
        // Stage Name
        _stageName = new TextUI(
            new Rectangle(
                (Singleton.SCREEN_WIDTH - textWidth) / 2, 
                (Singleton.SCREEN_HEIGHT - textHeight) / 2, 
                textWidth, 
                textHeight
            ),
            StageManager.GetStageName(),
            5,  
            Color.Gold, 
            TextUI.TextAlignment.Center
        );
        
        _ui.AddHUDElement(_stageName);
    }
    
    private void PrepareNextStage()
    {
        if(Singleton.Instance.Stage == 1)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
        }
        else
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;
        }
        ResetTransition();
    }

    private void ResetTransition()
    {
        _opacity = 0f;
        _fadeInTime = 1.0f;
        _messageTime = 2.5f;
        _timer = 0f;

        _currentState = TransitionState.SetUpText;
    }
}