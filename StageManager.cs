using System;
using System.Collections.Generic;

namespace FinalComGame;

public class StageManager
{
    private static readonly Dictionary<int, string[]> StageLayouts = new Dictionary<int, string[]>
    {
        {
            1, new[]
            {
                "../../../Data/Level_1/Level_1_Collision.csv",
            }
        },
        {
            2, new[]
            {
                "../../../Data/Level_1/Level_1_Collision.csv",
            }
        },
        {
            3, new[]
            {
                "../../../Data/Level_1/Level_1_Collision.csv",
            }
        },
        //Debug Stage
        {
            999, new[]
            {
                "../../../Data/Level_1/Level_1_Collision.csv",
            }
        },
        //Debug Stage
        {
            1000, new[]
            {
                "../../../Data/Level_1/Level_1_Collision.csv",
            }
        }
    };

    public static string GetCurrentStagePath()
    {
        if (!StageLayouts.ContainsKey(Singleton.Instance.Stage))
        {
            Console.WriteLine("No more stage : Replaying");
            Singleton.Instance.Stage = 1;
        }

        Console.WriteLine(Singleton.Instance.Stage);
        return StageLayouts[Singleton.Instance.Stage][0];

        // string[] layout = StageLayouts[Singleton.Instance.Stage];
        // int rows = layout.Length;
        // int cols = layout[0].Length;

        // for (int row = 0; row < rows; row++)
        // {
        //     for (int col = 0; col < cols; col++)
        //     {
        //         char chipChar = layout[row][col];
        //         Singleton.Instance.GameBoard[row, col] = ChipTypeFromChar(chipChar);
        //     }
        // }
    }

    // private static ChipType ChipTypeFromChar(char chipChar)
    // {
    //     return chipChar switch
    //     {
    //         'R' => ChipType.Red,
    //         'Y' => ChipType.Yellow,
    //         'B' => ChipType.Blue,
    //         'G' => ChipType.Green,
    //         'P' => ChipType.Purple,
    //         'W' => ChipType.White,
    //         'K' => ChipType.Black,
    //         'O' => ChipType.Orange,
    //         '-' => ChipType.None,
    //          _  => ChipType.None
    //     };
    // }
}
