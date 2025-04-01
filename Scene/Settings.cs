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
    private Button _musicSlideChip;
    private Button _sfxSlideChip;
    private Button _backButton;

    private int _pauseTitleHeight;
    private int _resumeButtonHeight;
    private int _settingsButtonHeight;
    private int _restartButtonHeight;
    private int _mainmenuButtonHeight;

    private int _musicLabelHeight;
    private int _musicSlideBarHeight;
    private int _musicSlideChipHeight;
    private int _sfxLabelHeight;
    private int _sfxSlideBarHeight;
    private int _sfxSlideChipHeight;
    private int _backButtonHeight;

    private float _musicSlideChipPosition;
    private float _sfxSlideChipPosition;

    private int _slideBarMaxValue;
    private int buttonGap;
    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(graphicsDevice, graphicsDeviceManager, content);

        _slideBarMaxValue = 320;

        // Y positon of the pause sign
        _pauseTitleHeight = 70;

        // Y positon of the resume button
        _resumeButtonHeight = 180;

        // a gap between each button
        buttonGap = 5;

        // // Calculating Y position of other buttons
        // for (int i = 0; i < 4; i++){
        //     switch (i)
        //     {
        //         case 1:
        //             _restartButtonHeight = _resumeButtonHeight + (buttonGap + ViewportManager.Get("Pause_Button").Height)*i;
        //             break;
        //         case 2:
        //             _settingsButtonHeight = _resumeButtonHeight + (buttonGap + ViewportManager.Get("Pause_Button").Height)*i;                
        //             break;
        //         case 3:
        //             _mainmenuButtonHeight = _resumeButtonHeight + (buttonGap + ViewportManager.Get("Pause_Button").Height)*i;
        //             break;    
        //         default:
        //             break;
        //     }
        // }

        // _musicLabelHeight = (Singleton.SCREEN_HEIGHT / 2) - (Singleton.CHIP_SIZE / 2) - (ViewportManager.Get("Big_Box0").Height/4) - 
        // (ViewportManager.Get("Slide_Bar").Height/2) - buttonGap;
        // _sfxLabelHeight = (Singleton.SCREEN_HEIGHT / 2) - (Singleton.CHIP_SIZE / 2) + (ViewportManager.Get("Big_Box0").Height/12) - 
        // (ViewportManager.Get("Slide_Bar").Height/2) - buttonGap;

        // _musicSlideBarHeight = (Singleton.SCREEN_HEIGHT / 2) - (ViewportManager.Get("Slide_Bar").Height / 2) - (ViewportManager.Get("Big_Box0").Height/4) + buttonGap;
        // _sfxSlideBarHeight = (Singleton.SCREEN_HEIGHT / 2) - (ViewportManager.Get("Slide_Bar").Height / 2) + (ViewportManager.Get("Big_Box0").Height/12) + buttonGap;

        // _musicSlideChipHeight = (Singleton.SCREEN_HEIGHT / 2) - (Singleton.CHIP_SIZE / 2) - (ViewportManager.Get("Big_Box0").Height/4) + buttonGap;
        // _sfxSlideChipHeight = (Singleton.SCREEN_HEIGHT / 2) - (Singleton.CHIP_SIZE / 2) + (ViewportManager.Get("Big_Box0").Height/12) + buttonGap;

        // _backButtonHeight = (Singleton.SCREEN_HEIGHT / 2) - (ViewportManager.Get("Small_Button").Height / 2) + (ViewportManager.Get("Big_Box0").Height*2/5);
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);
        SetupHUD();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

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
            Color.White, 
            TextUI.TextAlignment.Center
        );

        // Create a volume slider
        int SliderWidth = Singleton.SCREEN_WIDTH / 2;
        int SliderHeight = 30;
        Texture2D SliderBar = _content.Load<Texture2D>("ItemSlot");//TEMP
        Texture2D SliderHandle =   _content.Load<Texture2D>("sliderHandle"); // 32x32
        _musicVolumeSlider = new SlideBarUI(
            new Rectangle((Singleton.SCREEN_WIDTH - SliderWidth) / 2 , (Singleton.SCREEN_HEIGHT - SliderHeight) * 2 / 7, SliderWidth, SliderHeight),
            "Music Volume",
            SliderBar,
            SliderHandle,
            0,    // min value
            100,  // max value
            75,   // start value
            "{0}%" // value format
        );

        _soundEffectVolumeSlider = new SlideBarUI(
            new Rectangle((Singleton.SCREEN_WIDTH - SliderWidth) / 2 , (Singleton.SCREEN_HEIGHT - SliderHeight) * 3 / 7, SliderWidth, SliderHeight),
            "Sound Effect Volume",
            SliderBar,
            SliderHandle,
            0,    // min value
            100,  // max value
            75,   // start value
            "{0}%" // value format
        );

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later 
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;
        _backButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 6, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_backButton Button",
            Color.Wheat
        );
        _backButton.OnClick += BackButton_OnClick;

        _ui.AddHUDElement(_settingsTitle);
        _ui.AddHUDElement(_musicVolumeSlider);
        _ui.AddHUDElement(_soundEffectVolumeSlider);
        _ui.AddHUDElement(_backButton);
    }

    private void BackButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Pause; // Temp
    }
}
