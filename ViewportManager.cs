using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FinalComGame;

public class ViewportManager
{
    private static readonly Dictionary<string, Rectangle> _spriteRects = new Dictionary<string, Rectangle>
    {
        {"Grappling_Hook", new Rectangle(0, 0, 16, 16)},
        {"Gun_Bullet", new Rectangle(50, 5, 11, 5)},
        {"Staff_Bullet", new Rectangle(35, 17, 10, 13)},
        {"Charge_Bullet_0", new Rectangle(1, 5, 12, 5)},
        {"Charge_Bullet_1", new Rectangle(17, 4, 12, 7)},
        {"Charge_Bullet_2", new Rectangle(32, 3, 13, 9)},

        {"Player", new Rectangle(0, 0, 16, 32)},

        {"Slime", new Rectangle(0, 0, 16, 16)},
        {"Hellhound", new Rectangle(0, 0, 32, 32)},
        {"Skeleton", new Rectangle(0, 0, 16, 32)},
        {"PlatformEnemy", new Rectangle(0, 0, 48, 32)},
        {"TowerEnemy", new Rectangle(0, 0, 24, 24)},
        {"TowerEnemy_Bullet", new Rectangle(16, 16, 16, 16)},
        {"Demon", new Rectangle(0, 0, 16, 32)},
        {"Demon_Bullet", new Rectangle(4, 19, 8, 9)},

        {"GiantSlime", new Rectangle(0, 0, 64, 48)},
        {"Cerberus", new Rectangle(0, 0, 64, 48)},
        {"Rhulk", new Rectangle(0, 0, 32, 64)},
        {"Rhulk_Laser", new Rectangle(0, 0, 10, 200)},
        
        {"Potion_Health", new Rectangle(64, 64, 32, 32)},
        {"Potion_Speed", new Rectangle(0, 96, 32, 32)},
        {"Potion_Jump", new Rectangle(32, 64, 32, 32)},
        {"Barrier", new Rectangle(0, 64, 32, 32)},
        {"LifeUp", new Rectangle(32, 96, 32, 32)},
        {"Speed_Boots", new Rectangle(32, 32, 32, 32)},
        {"CursedGauntlet", new Rectangle(64, 32, 32, 32)},
        {"Sword", new Rectangle(32, 0, 32, 32)},
        {"Gun", new Rectangle(0, 0, 32, 32)},
        {"FireBall", new Rectangle(35, 17, 10, 13)},
        {"Fire_Staff", new Rectangle(64, 0, 32, 32)},
        {"Soul_Minion", new Rectangle(0, 0, 12, 12)},
        {"Soul_Bullet", new Rectangle(4, 51, 8, 9)},
        {"Soul_Staff", new Rectangle(0, 32, 32, 32)},
        {"Grenade", new Rectangle(64, 96, 32, 32)},
        {"Grenade_Projectile", new Rectangle(52, 17, 9, 13)},
        {"Explosion", new Rectangle(0, 0, 64, 64)},
    };
    
    public static Rectangle Get(string name){
        return _spriteRects[name];
    }
}