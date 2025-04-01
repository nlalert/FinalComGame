using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class PauseMenu : Scene
{
    private TextUI _pauseTitle; // can change to ImageUI later
    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainmenuButton;
    private Button _backButton;

    private Button _musicSlideChip;
    private Button _sfxSlideChip;

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

    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _pauseTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 7, TextWidth, TextHeight),
            "Pause",  
            Color.White, 
            TextUI.TextAlignment.Center
        );

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;

        _resumeButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Resume Button",
            Color.Wheat
        );
        _resumeButton.OnClick += ResumeButton_OnClick;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_restartButton Button",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _settingsButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_settingsButton Button",
            Color.Wheat
        );
        _settingsButton.OnClick += SettingButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_mainmenuButton Button",
            Color.Wheat
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _backButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 6 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_backButton Button",
            Color.Wheat
        );
        _backButton.OnClick += BackButton_OnClick;

        _ui.AddHUDElement(_pauseTitle);
        _ui.AddHUDElement(_resumeButton);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_settingsButton);
        _ui.AddHUDElement(_mainmenuButton);
        _ui.AddHUDElement(_backButton);

        // _musicSlideChip = new Button(_texture)
        // {
        //     Name = "MusicSlideChip",
        //     Viewport = ViewportManager.Get("Slide_Chip0"),
        //     Position = new Vector2((Singleton.SCREEN_WIDTH - ViewportManager.Get("Slide_Chip0").Width) / 2 + (int)(Singleton.Instance.MusicVolume*_slideBarMaxValue) - (_slideBarMaxValue/2),
        //             _musicSlideChipHeight),
        //     IsActive = true
        // };

        // _sfxSlideChip = new Button(_texture)
        // {
        //     Name = "SFXSlideChip",
        //     Viewport = ViewportManager.Get("Slide_Chip0"),
        //     Position = new Vector2((Singleton.SCREEN_WIDTH - ViewportManager.Get("Slide_Chip0").Width) / 2 + (int)(Singleton.Instance.SFXVolume*_slideBarMaxValue) - (_slideBarMaxValue/2),
        //             _sfxSlideChipHeight),
        //     IsActive = true
        // };

    }

    private void BackButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
    }

    private void MainMenuButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    private void SettingButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Settings;
    }

    private void RestartButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
    }

    private void ResumeButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PauseSong();
        // if (!_settings)
        // {

        //     // Unpause when left clicked & released on resume button or pressed & released "Escape key"
        //     if (_resumeButton.IsClicked() || (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) && Singleton.Instance.CurrentKey != Singleton.Instance.PreviousKey)){
        //         Singleton.Instance.CurrentGameState = Singleton.Instance.PreviousGameState;
        //     }

        //     // Restart to stage 1 when left clicked & released on restart button
        //     if (_restartButton.IsClicked())
        //     {
        //         Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
        //     }

        //     // Show settings when left clicked & released on settings button
        //     if (_settingsButton.IsClicked())
        //     {
        //         _settings = true;
        //     }

        //     // Exit to main-menu when left clicked & released on main-menu button
        //     if (_mainmenuButton.IsClicked())
        //     {
        //         Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
        //     }

        // }

        // else
        // {

        //     // Exit to pause menu when left clicked & released on back cutton or pressed & released "Escape key"
        //     if (_backButton.IsClicked() || Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape) && Singleton.Instance.CurrentKey != Singleton.Instance.PreviousKey)
        //     {
        //         _settings = false;
        //     }

        //     // Prevent dragging boyh chips at the same time
        //     if (!_sfxSlideChip.Dragging)
        //     {
        //     _musicSlideChip.Update(gameTime, _gameObjects);
        //     }
        //     if (!_musicSlideChip.Dragging)
        //     {
        //     _sfxSlideChip.Update(gameTime, _gameObjects);
        //     }

        //     // Adjust music volume by left click and drag the music slide chip
        //     if (_musicSlideChip.Dragging)
        //     {

        //         int newX = Math.Clamp(Singleton.Instance.CurrentMouseState.X - (_musicSlideChip.Viewport.Width / 2), (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) - _slideBarMaxValue/2, (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) + _slideBarMaxValue/2);
        //         _musicSlideChip.Position.X = newX;

        //         float _slideBarMinPosition = (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) - (_slideBarMaxValue/2);
        //         _musicSlideChipPosition = _musicSlideChip.Position.X - _slideBarMinPosition;
        //         Singleton.Instance.MusicVolume = _musicSlideChipPosition / _slideBarMaxValue;

        //     }
            
        //     // Adjust sfx volume by left click and drag the sfx slide chip
        //     if (_sfxSlideChip.Dragging)
        //     {

        //         int newX = Math.Clamp(Singleton.Instance.CurrentMouseState.X - (_sfxSlideChip.Viewport.Width / 2), (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) - _slideBarMaxValue/2, (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) + _slideBarMaxValue/2);
        //         _sfxSlideChip.Position.X = newX;

        //         float _slideBarMinPosition = (Singleton.SCREEN_WIDTH / 2) - (Singleton.CHIP_SIZE/ 2) - (_slideBarMaxValue/2);
        //         _sfxSlideChipPosition = _sfxSlideChip.Position.X - _slideBarMinPosition;
        //         //Singleton.Instance.Volume = _sfxSlideChipPosition / _slideBarMaxValue;

        //     }

        //     // Music slide chip base on music volume
        //     if (Singleton.Instance.MusicVolume <= 0)
        //     {
        //         _musicSlideChip.Viewport = ViewportManager.Get("Slide_Chip0");
        //     }
        //     else if (Singleton.Instance.MusicVolume <= 0.33)
        //     {
        //         _musicSlideChip.Viewport = ViewportManager.Get("Slide_Chip1");
        //     }
        //     else if (Singleton.Instance.MusicVolume <= 0.66)
        //     {
        //         _musicSlideChip.Viewport = ViewportManager.Get("Slide_Chip2");
        //     }
        //     else
        //     {
        //         _musicSlideChip.Viewport = ViewportManager.Get("Slide_Chip3");             
        //     }

        //     // SFX slide chip base on SFX volume
        //     if (Singleton.Instance.SFXVolume <= 0)
        //     {
        //         _sfxSlideChip.Viewport = ViewportManager.Get("Slide_Chip0");
        //     }
        //     else if (Singleton.Instance.SFXVolume <= 0.33)
        //     {
        //         _sfxSlideChip.Viewport = ViewportManager.Get("Slide_Chip1");
        //     }
        //     else if (Singleton.Instance.SFXVolume <= 0.66)
        //     {
        //         _sfxSlideChip.Viewport = ViewportManager.Get("Slide_Chip2");
        //     }
        //     else
        //     {
        //         _sfxSlideChip.Viewport = ViewportManager.Get("Slide_Chip3");             
        //     }

        // }
    }

    // public void Draw(GameTime gameTime)
    // {
        // // Tranparent background
        // _spriteBatch.Draw(_rectTexture, Vector2.Zero, new Rectangle(0, 0, Singleton.SCREEN_WIDTH, Singleton.SCREEN_HEIGHT), new Color(0, 0, 0, 150));

        // if (!_settings)
        // {         
        //     // Pause Title
        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("Pause_Title0").Width / 2), 
        //     _pauseTitleHeight - (ViewportManager.Get("Pause_Title0").Height / 2)), ViewportManager.Get("Pause_Title0"), Color.White);

        //     // Buttons
        //     _resumeButton.Draw(_spriteBatch);
        //     _restartButton.Draw(_spriteBatch);
        //     _settingsButton.Draw(_spriteBatch);
        //     _mainmenuButton.Draw(_spriteBatch);

        // }

        // else {

        //     // Settings Box
        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("Big_Box0").Width/ 2),
        //      (Singleton.SCREEN_HEIGHT / 2) - (ViewportManager.Get("Big_Box0").Height/ 2)), ViewportManager.Get("Big_Box0"), Color.White);
            
        //     // Music
        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("Music_Label").Width/ 2),
        //     _musicLabelHeight), ViewportManager.Get("Music_Label"), Color.White);

        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("Slide_Bar").Width/ 2),
        //     _musicSlideBarHeight), ViewportManager.Get("Slide_Bar"), Color.White);
            
        //     // SFX
        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("SFX_Label").Width/ 2),
        //     _sfxLabelHeight), ViewportManager.Get("SFX_Label"), Color.White);

        //     _spriteBatch.Draw(_texture, new Vector2((Singleton.SCREEN_WIDTH / 2) - (ViewportManager.Get("Slide_Bar").Width/ 2),
        //     _sfxSlideBarHeight), ViewportManager.Get("Slide_Bar"), Color.White);
            
        //     // Back button
        //     _backButton.Draw(_spriteBatch);

        //     // Slide Chip
        //     _musicSlideChip.Draw(_spriteBatch);
        //     _sfxSlideChip.Draw(_spriteBatch);
        // }
    // }
}
