using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Particle : GameObject
    {
        private int _count;
        private Vector2 _center;

        private Vector2[] _position;

        private float[] _size;
        private float[] _tiltDegree;
        private int[] _tiltDirection;

        private float[] _tiltSpeed;
        private float[] _floatSpeed;
        private float[] _sideSpeed;
        
        public Particle (int count, Vector2 position, Texture2D texture)
        {

            _count = count;
            _center = new Vector2 (position.X + 8, position.Y + 4);
            _texture = texture;

            _position = new Vector2[_count];

            _size = new float[_count];
            _tiltDegree = new float[_count];
            _tiltDirection = new int[_count];

            _tiltSpeed = new float[_count];
            _floatSpeed = new float[_count];
            _sideSpeed = new float[_count];
            
            for (int i = 0; i < count; i++)
            {
                _position[i] = new Vector2( _center.X + ((float)Singleton.Instance.Random.NextDouble() * 4 - 2) , 
                                            _center.Y + ((float)Singleton.Instance.Random.NextDouble() * 4 - 2) );

                _size[i] = (float)Singleton.Instance.Random.NextDouble() + 1;
                _tiltDegree[i] = Singleton.Instance.Random.Next(0,90);
                _tiltDirection[i] = Singleton.Instance.Random.Next(2) * 2 - 1 ;

                _tiltSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.05f + 0.01f;
                _floatSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.1f + 0.5f;
                _sideSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.2f - 0.1f;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _count; i ++)
            {
                spriteBatch.Draw(
                    _texture,
                    _position[i],
                    new Rectangle(0, 0, (int)_size[i], (int)_size[i]),
                    Color.White,
                    _tiltDegree[i], 
                    new Vector2((int)_size[i]/2, (int)_size[i]/2),
                    _size[i],
                    SpriteEffects.None, 
                    0f
                );
            }
        }

        public override void Reset()
        {

        }

        public void Update(Vector2 position)
        {
            _center = new Vector2 (position.X + 8, position.Y + 4);

            for (int i = 0; i < _count; i ++)
            {
                _size[i] -= 0.02f;

                if (_size[i] > 1){
                    _position[i].Y -= _floatSpeed[i];
                    _position[i].X += _sideSpeed[i];
                    _tiltDegree[i] += _tiltSpeed[i] * _tiltDirection[i];
                }

                else
                {
                    _position[i] = new Vector2( _center.X + ((float)Singleton.Instance.Random.NextDouble() * 4 - 2) , 
                                                _center.Y + ((float)Singleton.Instance.Random.NextDouble() * 4 - 2) );

                    _size[i] = (float)Singleton.Instance.Random.NextDouble() + 1;
                    _tiltDegree[i] = Singleton.Instance.Random.Next(0,90);
                    _tiltDirection[i] = Singleton.Instance.Random.Next(2) * 2 - 1 ;

                    _tiltSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.05f + 0.01f;
                    _floatSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.1f + 0.4f;
                    _sideSpeed[i] = (float)Singleton.Instance.Random.NextDouble() * 0.2f - 0.1f;
                }
            }
        }
    }

}