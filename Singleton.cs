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
    public const int TERMINAL_VELOCITY = 500;
    public const int MAX_VISIBLE_PROMPTS = 5;
    public const int COLLISION_RADIUS = 4;
    public const int UPDATE_DISTANCE = 50 * TILE_SIZE;
    public Random Random;
    public int Stage;
    public Player Player;
    public Camera Camera;
    public UI CurrentUI;

    public float MusicVolume = 1.0f;
    public float SFXVolume = 1.0f;

    public enum GameState
    {
        MainMenu,
        StartingGame,
        InitializingStage,
        Playing,
        Pause,
        Settings,
        StageCompleted,
        ChangingStage,
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

    // Enum for mouse buttons
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    // Checks if a mouse button is being held down
    public bool IsMouseButtonPressed(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentMouseState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentMouseState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return CurrentMouseState.MiddleButton == ButtonState.Pressed;
            default:
                return false;
        }
    }

    // Checks if a mouse button was JUST pressed (prevents holding issues)
    public bool IsMouseButtonJustPressed(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentMouseState.LeftButton == ButtonState.Pressed && 
                    PreviousMouseState.LeftButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentMouseState.RightButton == ButtonState.Pressed && 
                    PreviousMouseState.RightButton == ButtonState.Released;
            case MouseButton.Middle:
                return CurrentMouseState.MiddleButton == ButtonState.Pressed && 
                    PreviousMouseState.MiddleButton == ButtonState.Released;
            default:
                return false;
        }
    }

    // Checks if a mouse button was JUST released
    public bool IsMouseButtonJustReleased(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentMouseState.LeftButton == ButtonState.Released && 
                    PreviousMouseState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentMouseState.RightButton == ButtonState.Released && 
                    PreviousMouseState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return CurrentMouseState.MiddleButton == ButtonState.Released && 
                    PreviousMouseState.MiddleButton == ButtonState.Pressed;
            default:
                return false;
        }
    }

    // Checks if a mouse button is released
    public bool IsMouseButtonReleased(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentMouseState.LeftButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentMouseState.RightButton == ButtonState.Released;
            case MouseButton.Middle:
                return CurrentMouseState.MiddleButton == ButtonState.Released;
            default:
                return false;
        }
    }

    // Gets the current mouse position
    public Point GetMousePosition()
    {
        return CurrentMouseState.Position;
    }

    // Gets the mouse position delta since last frame
    public Point GetMousePositionDelta()
    {
        return new Point(
            CurrentMouseState.Position.X - PreviousMouseState.Position.X,
            CurrentMouseState.Position.Y - PreviousMouseState.Position.Y
        );
    }

    // Gets the mouse scroll wheel value
    public int GetScrollWheelValue()
    {
        return CurrentMouseState.ScrollWheelValue;
    }

    // Gets the mouse scroll wheel delta since last frame
    public int GetScrollWheelDelta()
    {
        return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
    }
}

