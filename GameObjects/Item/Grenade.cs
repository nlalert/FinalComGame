using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Grenade : Item
{
    // Projectile template
    public GrenadeProjectile GrenadeProjectile;
    
    public Grenade(Texture2D texture, ItemType type, Vector2 Position)
        : base(texture, Position, type)
    {
    }
    
    public override void ActiveAbility(float deltaTime, int slot, List<GameObject> gameObjects)
    {
        // Throw the grenade
        ThrowGrenade(gameObjects);
        
        // Deactivate after throwing
        IsActive = false;
    }
    
    private void ThrowGrenade(List<GameObject> gameObjects)
    {
        GrenadeProjectile newGrenade = GrenadeProjectile.Clone() as GrenadeProjectile;
        newGrenade.Shoot(Singleton.Instance.Player.Position, new Vector2(Singleton.Instance.Player.Direction, (float)Math.Sin(MathHelper.ToRadians(-45))));
        gameObjects.Add(newGrenade);
        // return newFireball;
        // // Clone the projectile template
        // GrenadeProjectile grenade = GrenadeProjectile.Clone() as GrenadeProjectile;
        // if (grenade == null) return;
        
        // Player player = Singleton.Instance.Player;
        
        // // Set starting position (adjust based on player's position and state)
        // Vector2 startPos = player.Position;
        // if (player._isCrouching)
        //     startPos += new Vector2(player.Direction * 20, 6); // Lower when crouching
        // else
        //     startPos += new Vector2(player.Direction * 20, 16); // At hand level
        
        // grenade.Position = startPos;
        
        // // Calculate throw velocity
        // float throwAngle = player.Direction > 0 ? ThrowAngle : MathHelper.Pi - ThrowAngle;
        // Vector2 throwDirection = new Vector2(
        //     (float)Math.Cos(throwAngle),
        //     (float)Math.Sin(throwAngle)
        // );
        
        // // Adjust throw power based on player state
        // float speedMultiplier = player._isCrouching ? 0.7f : 1.0f;
        
        // // Set velocity
        // grenade.Velocity = throwDirection * ThrowSpeed * speedMultiplier;
        
        // // Play throw sound
        // if (ThrowSound != null)
        //     ThrowSound.Play();
        
        // // Start detonation timer
        // grenade.DetonateTimer = grenade.DetonateDelayDuration;
        
        // // Add to game objects
        // gameObjects.Add(grenade);
    }
}

