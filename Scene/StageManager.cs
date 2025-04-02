using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FinalComGame;

public class StageManager
{
    private static readonly Dictionary<int, Vector2> PlayerGridSpawnPoint = new Dictionary<int, Vector2>
    {
        { 0, new Vector2(10, 38) },
        { 1, new Vector2(10, 90) },
        { 2, new Vector2(16, 10) },
        { 3, new Vector2(16, 15) },

        //Debug Stage
        { 999, new Vector2(0, 0) },
        { 1000, new Vector2(0, 0) }
    };

    public static Vector2 GetPlayerWorldSpawnPosition()
    {
        return TileMap.GetTileWorldPositionAt(PlayerGridSpawnPoint[Singleton.Instance.Stage]);
    }

    public static string GetCurrentStageCollisionPath()
    {
        if (!PlayerGridSpawnPoint.ContainsKey(Singleton.Instance.Stage))
        {
            Console.WriteLine("No more stage : Replaying");
            Singleton.Instance.Stage = 1;
        }

        return "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_Collision.csv";
    }
}