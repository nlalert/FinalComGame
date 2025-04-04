using System;
using System.Collections.Generic;
using System.Linq;
using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameObject : ICloneable
{
    protected Texture2D _texture;

    public Vector2 Position;

    public float Radius;

    public float Rotation;
    public Vector2 Scale;

    public Vector2 Velocity;

    public string Name;

    public bool IsActive;
    public Animation Animation;

    public Rectangle Rectangle
    {
        get
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)(Viewport.Width * Scale.X), (int)(Viewport.Height * Scale.Y)) ;
        }
    }

    public Rectangle Viewport;

    public GameObject()
    {
        Position = Vector2.Zero;
        Scale = Vector2.One;
        Rotation = 0f;
        IsActive = true;
    }

    public GameObject(Texture2D texture)
    {
        _texture = texture;
        Position = Vector2.Zero;
        Scale = Vector2.One;
        Rotation = 0f;
        IsActive = true;
    }

    public virtual void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    public virtual void Reset()
    {

    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    #region Collision
    protected bool IsTouching(GameObject g)
    {
        return  IsTouchingLeft(g)   ||
                IsTouchingTop(g)    ||
                IsTouchingRight(g)  ||
                IsTouchingBottom(g);
    }

    protected bool IsTouching(Rectangle hitbox)
    {
        return  IsTouchingLeft(hitbox)   ||
                IsTouchingTop(hitbox)    ||
                IsTouchingRight(hitbox)  ||
                IsTouchingBottom(hitbox);
    }

    protected bool IsTouchingLeft(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left &&
                this.Rectangle.Left < g.Rectangle.Left  &&
                this.Rectangle.Bottom > g.Rectangle.Top &&
                this.Rectangle.Top < g.Rectangle.Bottom;
    }

    protected bool IsTouchingLeft(Rectangle hitbox)
    {
        return  this.Rectangle.Right > hitbox.Left &&
                this.Rectangle.Left < hitbox.Left  &&
                this.Rectangle.Bottom > hitbox.Top &&
                this.Rectangle.Top < hitbox.Bottom;
    }

    protected bool IsTouchingRight(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Right  &&
                this.Rectangle.Left < g.Rectangle.Right   &&
                this.Rectangle.Bottom > g.Rectangle.Top   &&
                this.Rectangle.Top < g.Rectangle.Bottom;
    }

    protected bool IsTouchingRight(Rectangle hitbox)
    {
        return  this.Rectangle.Right > hitbox.Right  &&
                this.Rectangle.Left < hitbox.Right   &&
                this.Rectangle.Bottom > hitbox.Top   &&
                this.Rectangle.Top < hitbox.Bottom;
    }

    protected bool IsTouchingTop(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left &&
                this.Rectangle.Left < g.Rectangle.Right &&
                this.Rectangle.Bottom >= g.Rectangle.Top &&
                this.Rectangle.Top < g.Rectangle.Top;
    }

    protected bool IsTouchingTop(Rectangle hitbox)
    {
        return  this.Rectangle.Right > hitbox.Left &&
                this.Rectangle.Left < hitbox.Right &&
                this.Rectangle.Bottom >= hitbox.Top &&
                this.Rectangle.Top < hitbox.Top;
    }

    protected bool IsTouchingBottom(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left     &&
                this.Rectangle.Left < g.Rectangle.Right     &&
                this.Rectangle.Bottom > g.Rectangle.Bottom  &&
                this.Rectangle.Top < g.Rectangle.Bottom;
    }

    protected bool IsTouchingBottom(Rectangle hitbox)
    {
        return  this.Rectangle.Right > hitbox.Left     &&
                this.Rectangle.Left < hitbox.Right     &&
                this.Rectangle.Bottom > hitbox.Bottom  &&
                this.Rectangle.Top < hitbox.Bottom;
    }

    protected bool ResolveHorizontalCollision(GameObject g)
    {
        bool isCollided = false;
        if (IsTouchingLeft(g))
        {
            if (this.Velocity.X > 0) // Moving right
            {
                this.Position.X = g.Rectangle.Left - this.Rectangle.Width;
                this.Velocity.X = 0;
                isCollided = true;
            }
        }
        if(IsTouchingRight(g))
        {            
            if (this.Velocity.X < 0) // Moving left
            {
                this.Position.X = g.Rectangle.Right;
                this.Velocity.X = 0;
                isCollided = true;
            }
        }
        return isCollided;
    }

    protected bool ResolveVerticalCollision(GameObject g)
    {
        bool isCollided = false;
        if (IsTouchingTop(g))
        {
            if (this.Velocity.Y > 0) // Falling down
            {
                this.Position.Y = g.Rectangle.Top - this.Rectangle.Height;
                this.Velocity.Y = 0;
                isCollided = true;
            }
        }
        if(IsTouchingBottom(g))
        {
            if (this.Velocity.Y < 0) // Moving up
            {
                this.Position.Y = g.Rectangle.Bottom;
                this.Velocity.Y = 0;
                isCollided = true;
            }
        }
        return isCollided;
    }


    #endregion

    protected virtual void ApplyGravity(float deltaTime)
    {
        Velocity.Y += Singleton.GRAVITY * deltaTime; // gravity
        
        ClampMaxFallVelocity();
    }

    protected void ClampMaxFallVelocity()
    {
        if(Velocity.Y >= Singleton.TERMINAL_VELOCITY)
        {
            Velocity.Y = Singleton.TERMINAL_VELOCITY;
        }
    }
    
    protected virtual void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
    {
        Position.X += Velocity.X * deltaTime;

        for (int i = -Singleton.COLLISION_RADIUS; i <= Singleton.COLLISION_RADIUS; i++)
        {
            for (int j = -Singleton.COLLISION_RADIUS; j <= Singleton.COLLISION_RADIUS; j++)
            {
                Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                if(tile != null && tile.IsSolid)
                {
                    ResolveHorizontalCollision(tile);
                }
            }
        }
        foreach (var platformEnemy in gameObjects.OfType<PlatformEnemy>())
        {
            ResolveHorizontalCollision(platformEnemy);
        }
    }

    protected virtual void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
    {
        Position.Y += Velocity.Y * deltaTime;

        for (int i = -Singleton.COLLISION_RADIUS; i <= Singleton.COLLISION_RADIUS; i++)
        {
            for (int j = -Singleton.COLLISION_RADIUS; j <= Singleton.COLLISION_RADIUS; j++)
            {
                Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                if(tile != null && tile.IsSolid)
                {
                    ResolveVerticalCollision(tile);
                }
                
            }
        }
        foreach (var platformEnemy in gameObjects.OfType<PlatformEnemy>())
        {
            ResolveVerticalCollision(platformEnemy);
        }
    }
}
