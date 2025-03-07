using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class GameObject : ICloneable
{
    protected Texture2D _texture;

    public Vector2 Position;

    public int Radius;

    public float Rotation;
    public Vector2 Scale;

    public Vector2 Velocity;

    public string Name;

    public bool IsActive;

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

    public virtual void Update(GameTime gameTime, List<GameObject> gameObjects)
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

    protected bool IsTouchingLeft(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left &&
                this.Rectangle.Left < g.Rectangle.Left  &&
                this.Rectangle.Bottom > g.Rectangle.Top &&
                this.Rectangle.Top < g.Rectangle.Bottom;
    }

    protected bool IsTouchingRight(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Right  &&
                this.Rectangle.Left < g.Rectangle.Right   &&
                this.Rectangle.Bottom > g.Rectangle.Top   &&
                this.Rectangle.Top < g.Rectangle.Bottom;
    }

    protected bool IsTouchingTop(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left &&
                this.Rectangle.Left < g.Rectangle.Right &&
                this.Rectangle.Bottom >= g.Rectangle.Top &&
                this.Rectangle.Top < g.Rectangle.Top;
    }

    protected bool IsTouchingBottom(GameObject g)
    {
        return  this.Rectangle.Right > g.Rectangle.Left     &&
                this.Rectangle.Left < g.Rectangle.Right     &&
                this.Rectangle.Bottom > g.Rectangle.Bottom  &&
                this.Rectangle.Top <= g.Rectangle.Bottom;
    }

    protected void ResolveHorizontalCollision(GameObject g)
    {
        if (IsTouchingLeft(g))
        {
            if (Velocity.X > 0) // Moving right
            {
                Position.X = g.Rectangle.Left - Rectangle.Width;
                Velocity.X = 0;
                Console.WriteLine("touch left");
            }
        }
        if(IsTouchingRight(g))
        {            
            if (Velocity.X < 0) // Moving left
            {
                Position.X = g.Rectangle.Right;
                Velocity.X = 0;
                Console.WriteLine("touch right");
            }
        }
    }

    protected void ResolveVerticalCollision(GameObject g)
    {
        if (IsTouchingTop(g))
        {
            if (Velocity.Y > 0) // Falling down
            {
                Position.Y = g.Rectangle.Top - Rectangle.Height;
                // Console.WriteLine("SnapUp");
                // Console.WriteLine("g.Rectangle.Top: "+g.Rectangle.Top);
                Velocity.Y = 0;
            }
        }
        if(IsTouchingBottom(g))
        {
            if (Velocity.Y < 0) // Moving up
            {
                Position.Y = g.Rectangle.Bottom;
                Velocity.Y = 0;
            }
        }
    }

    protected bool IsTouchingAsCircle(GameObject g)
    {
        float distance = Vector2.Distance(this.Position, g.Position);
        return distance < this.Radius + g.Radius;
    }

    #endregion
}
