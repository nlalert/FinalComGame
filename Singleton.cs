using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Singleton
{
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
}

