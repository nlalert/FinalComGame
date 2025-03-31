using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

class Singleton
{
    public const int SCREEN_HEIGHT = 720;
    public const int SCREEN_WIDTH = 1280;
    public const int TILE_SIZE = 16;
    public const int GRAVITY = 2000;
    public Random Random;
    public int Stage;
    public Player Player;
    public Camera Camera;
    public UI CurrentUI;

    public enum GameState
    {
        MainMenu,
        StartingGame,
        InitializingStage,
        Playing,
        Pause,
        StageCompleted,
        GameOver,
        GameWon,
        Exit,
    }

    public GameState CurrentGameState;

    public KeyboardState PreviousKey, CurrentKey;
    public MouseState PreviousMouseState, CurrentMouseState;
    public SpriteFont GameFont;
    private static Singleton instance;

    private Singleton() { 
        Random = new Random(); 
    }

    public static Singleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Singleton();
            }
            return instance;
        }
    }
    
    public void UpdateCurrentInput()
    {
        CurrentKey = Keyboard.GetState();
        CurrentMouseState = Mouse.GetState();
    }

    public void UpdatePreviousInput()
    {
        PreviousKey = CurrentKey;
        PreviousMouseState = CurrentMouseState;
    }

    // Checks if a key is being held down
    public bool IsKeyPressed(Keys key)
    {
        return CurrentKey.IsKeyDown(key);
    }

    // Checks if a key was JUST pressed (prevents holding issues)
    public bool IsKeyJustPressed(Keys key)
    {
        return CurrentKey.IsKeyDown(key) && PreviousKey.IsKeyUp(key);
    }

    // Checks if a key was JUST released
    public bool IsKeyJustReleased(Keys key)
    {
        return CurrentKey.IsKeyUp(key) && PreviousKey.IsKeyDown(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return CurrentKey.IsKeyUp(key);
    }

}

