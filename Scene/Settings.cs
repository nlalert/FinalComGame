using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace FinalComGame;

public class Settings : Scene
{
    private TextUI _settingsTitle; // can change to ImageUI later
    private SlideBarUI _musicVolumeSlider;
    private SlideBarUI _soundEffectVolumeSlider;
    private Button _backButton;
    private Texture2D _UITexture;
    private Rectangle _buttonRectangle;

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        _UITexture = _content.Load<Texture2D>("UI");
        _buttonRectangle = new Rectangle(0, 48, 304, 48);
        base.LoadContent(spriteBatch);
        SetupHUD();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _gameManager.IsMouseVisible = true;

        Singleton.Instance.MusicVolume = _musicVolumeSlider.GetValue()/100;
        Singleton.Instance.SFXVolume = _soundEffectVolumeSlider.GetValue()/100;

        MediaPlayer.Volume = Singleton.Instance.MusicVolume; 
        SoundEffect.MasterVolume = Singleton.Instance.SFXVolume;
    }

    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _settingsTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 7, TextWidth, TextHeight),
            "Settings",  
            4,
            Color.White, 
            TextUI.TextAlignment.Center
        );

        // Create a volume slider
        int SliderWidth = Singleton.SCREEN_WIDTH / 2;
        int SliderHeight = 30;
        // Texture2D SliderBar = _content.Load<Texture2D>("");//TEMP
        // Texture2D SliderHandle =   _content.Load<Texture2D>("sliderHandle");
        _musicVolumeSlider = new SlideBarUI(
            new Rectangle((Singleton.SCREEN_WIDTH - SliderWidth) / 2 , (Singleton.SCREEN_HEIGHT - SliderHeight) * 2 / 7, SliderWidth, SliderHeight),
            "Music Volume",
            _UITexture,
            0,    // min value
            100,  // max value
            75,   // start value
            "{0:F0}%" // value format
        );

        _soundEffectVolumeSlider = new SlideBarUI(
            new Rectangle((Singleton.SCREEN_WIDTH - SliderWidth) / 2 , (Singleton.SCREEN_HEIGHT - SliderHeight) * 3 / 7, SliderWidth, SliderHeight),
            "SFX Volume",
            _UITexture,
            0,    // min value
            100,  // max value
            75,   // start value
            "{0:F0}%" // value format
        );

        int ButtonWidth = Singleton.SCREEN_WIDTH / 3;
        int ButtonHeight = 80;
        _backButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 7, ButtonWidth, ButtonHeight),
            _UITexture,
            "Back",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _backButton.OnClick += BackButton_OnClick;

        _ui.AddHUDElement(_settingsTitle);
        _ui.AddHUDElement(_musicVolumeSlider);
        _ui.AddHUDElement(_soundEffectVolumeSlider);
        _ui.AddHUDElement(_backButton);
    }

    private void BackButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Pause;
    }
}
