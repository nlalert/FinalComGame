using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;
public class SoulStaff : Item
{
    public float MPCost;
    public SoulMinion soulMinion;
    private bool isSoulSummon;
    public SoulStaff(Texture2D texture, ItemType type, Vector2 Position, float jumpModifier = 1.5f, float jumpBoostDuration = 5.0f)
            : base(texture, Position, type)
    {
        
    }
    public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
    {
        //Spawn Soul
        if(!isSoulSummon){
            SoulMinion bullet = soulMinion.Clone() as SoulMinion;
            Vector2 bulletPosition = Singleton.Instance.Player.Position;
            bullet.DamageAmount = bullet.BaseDamageAmount;
            bullet.Shoot(bulletPosition, Vector2.Zero);
            bullet.IsActive =true;
            gameObjects.Add(bullet);
            Console.WriteLine("Soul is Spawn");
            isSoulSummon = true;
        }
        
        base.ActiveAbility(deltaTime, slot, gameObjects);
    }
    public override void Use(int slot)
    {
        isSoulSummon = false;
        base.Use(slot);
    }

}