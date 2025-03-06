using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Singleton
{
    public const int SCREEN_HEIGHT = 700;
    public const int SCREEN_WIDTH = 700;
    public const int BLOCK_SIZE = 32;
    public const int GRAVITY = 5000;
    public Random Random;

    public enum GameState
    {
        MainMenu,
        SetLevel,
        Playing,
        CheckChipAndCeiling,
        Pause,
        GameOver,
    }

    public GameState CurrentGameState;

    public KeyboardState PreviousKey, CurrentKey;
    public MouseState PreviousMouseState,CurrentMouseState;
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

}

