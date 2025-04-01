using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Grenade : Item
{
    public GrenadeProjectile GrenadeProjectile;
    
    public Grenade(Texture2D texture, ItemType type, Vector2 Position)
        : base(texture, Position, type)
    {
    }
    
    public override void ActiveAbility(float deltaTime, int slot, List<GameObject> gameObjects)
    {
        RemoveItem();
        ThrowGrenade(gameObjects);
    }
    
    private void ThrowGrenade(List<GameObject> gameObjects)
    {
        GrenadeProjectile newGrenade = GrenadeProjectile.Clone() as GrenadeProjectile;
        newGrenade.Shoot(Singleton.Instance.Player.Position, new Vector2(Singleton.Instance.Player.Direction, (float)Math.Sin(MathHelper.ToRadians(-45))));
        gameObjects.Add(newGrenade);
    }
}

