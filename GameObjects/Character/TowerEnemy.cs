using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TowerEnemy : BaseEnemy
    {
        private float shootCooldown = 5.0f;
        private float shootTimer;
        private Rectangle _baseTextureViewPort;
        public TowerBullet TowerBullet;
        public SoundEffect Tower_sound;

        public TowerEnemy(Texture2D texture) : base(texture)
        {
            _texture = texture;
            DetectionRange = 350f;
            AttackRange = 900f;
            CanCollideTile = false;
        }
        public override void Reset()
        {
            shootCooldown = 5.0f;
            base.Reset();
        }
        public override void OnSpawn()
        {
            Position -= new Vector2(4,4);
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateInvincibilityTimer(deltaTime);
            shootTimer += deltaTime;
            if (CurrentState == EnemyState.Idle && shootTimer >= shootCooldown * 4/5)
            {
                shootTimer = shootCooldown * 4/5;
            }
            UpdateHorizontalMovement(deltaTime,gameObjects,tileMap);
            UpdateVerticalMovement(deltaTime,gameObjects,tileMap);
            
            if (Singleton.Instance.Player != null)
            {
                switch (CurrentState)
                {
                    case EnemyState.Idle:
                        AI_Idle(gameTime,gameObjects,tileMap,deltaTime);
                        break;

                    case EnemyState.Chase:
                        AI_Chase(gameTime,gameObjects,tileMap);
                        break;
                }
            }

            UpdateAnimation(deltaTime);
                
            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void AddAnimation(){
            Animation = new Animation(_texture, 48, 48, new Vector2(48*8, 48*4), 12);

            Animation.AddAnimation("idle", new Vector2(0, 1), 4);
            Animation.AddAnimation("charge_1", new Vector2(0, 0), 4);
            Animation.AddAnimation("charge_2", new Vector2(4, 0), 4);

            Animation.AddAnimation("shoot", new Vector2(0, 2), 11);

            Animation.AddAnimation("die", new Vector2(4, 3), 4);
            Animation.AddAnimation("base", new Vector2(7, 1), 1);

            Animation.ChangeAnimation("base");
            _baseTextureViewPort = Animation.GetCurrentFrame();

            Animation.ChangeAnimation("idle");
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (!Animation.IsTransition){

                if (shootTimer == 0){
                    animation = "shoot";
                }
                else if (shootTimer >= shootCooldown* 2/3)
                    animation = "idle";

                else if (shootTimer >= shootCooldown*1/3)
                    animation = "charge_2";

                else
                    animation = "charge_1";
            }
   

            if (CurrentState == EnemyState.Idle && animation == "idle")
            {
                Animation.SetFPS(12);
            }

            else
            {
                Animation.SetFPS(24);
            }
                

            if(_currentAnimation != animation && !Animation.IsTransition)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    case "shoot":
                        Animation.ChangeTransitionAnimation(_currentAnimation, "charge_1");
                        break;
                    case "charge_1":
                    case "charge_2":
                        Animation.ChangeAnimationAndKeepFrame(_currentAnimation);
                        break;
                    case "idle":
                        Animation.ChangeAnimationAndKeepFrame(_currentAnimation);
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
     
            base.UpdateAnimation(deltaTime);
        }

        private void AI_Idle(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap, float deltaTime){
            if (HaveLineOfSightOfPlayer(tileMap))
            {
                // Transition to chase state
                CurrentState = EnemyState.Chase;
            }
        }
        private void AI_Chase(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            float distanceToPlayer = Vector2.Distance(Position, Singleton.Instance.Player.GetPlayerCenter());
            if (!HaveLineOfSightOfPlayer(tileMap))
            {
                CurrentState = EnemyState.Idle;
                Velocity = Vector2.Zero; // Stop moving
            }
            else
            {
                if (distanceToPlayer <=AttackRange && shootTimer >= shootCooldown && HaveLineOfSightOfPlayer(tileMap))
                {
                    Tower_sound.Play();
                    ShootBullet(gameObjects);
                    shootTimer = 0;
                }
            }
        }
        private void ShootBullet(List<GameObject> gameObjects)
        {
            Vector2 direction = Vector2.Normalize(Singleton.Instance.Player.Position - Position);
            TowerBullet bullet = TowerBullet.Clone() as TowerBullet;

            Vector2 bulletPosition = new Vector2(Position.X + Viewport.Width / 2, Position.Y + Viewport.Height / 2);

            bullet.DamageAmount = bullet.BaseDamageAmount;
            bullet.Shoot(bulletPosition, direction);
            gameObjects.Add(bullet);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(Animation.GetCurrentFrame().Width / 2, Animation.GetCurrentFrame().Height / 2);
            Vector2 direction = Singleton.Instance.Player.Position - Position;
            Color color = IsInvincible() ? Color.Red : Color.White;
            
            float rotation = 0.0f;
            if (CurrentState == EnemyState.Chase) {
                rotation = MathF.Atan2(direction.Y, direction.X) + MathF.PI / 2;
                // Console.WriteLine(rotation);
            }

            {
                spriteBatch.Draw(
                    Animation.GetTexture(),
                    GetDrawingPosition(),
                    _baseTextureViewPort,
                    Color.White,
                    0f, 
                    Vector2.Zero,
                    Scale,
                    SpriteEffects.None, 
                    0f
                );
            }

            // rotation = 0f;
            // center = Vector2.Zero;

            {
                spriteBatch.Draw(
                    Animation.GetTexture(),
                    GetDrawingPosition() + center,
                    Animation.GetCurrentFrame(),
                    color,
                    rotation, 
                    center,
                    Scale,
                    SpriteEffects.None, 
                    0f
                );
            }

            //base.Draw(spriteBatch);
            //DrawDebug(spriteBatch);
        }
        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\n HP{Health}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
        }
    }
}
