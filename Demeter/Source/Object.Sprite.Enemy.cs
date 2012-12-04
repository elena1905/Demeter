using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    class Enemy : Sprite
    {
        #region Logical
        public override int CollisionWidth
        {
            get { return 45; }
        }

        public override int CollisionHeight
        {
            get { return 39; }
        }
        #endregion

        #region Drawing

        public override int TopCollisionOffset
        {
            get { return 0; }
        }

        public override int LeftCollisionOffset
        {
            get { return 0; }
        }

        static readonly Point DEFAULT_FRAME_SIZE = new Point(45, 39);

        // animations

        Animation runAnimation;
        Animation dieAnimation;

        int dieTime = 0;
        #endregion

        // Constants for controling horizontal movement
        private const float horizontalMoveSpeed = 1.5f;
        // Constants for controlling vertical movement
        private const float MaxFallSpeed = 18.0f;
        private const float GravityAcceleration = 20.0f;

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        Vector2 speed = Vector2.Zero;

        bool face_right;// the speed is positive or not

        float leftBound;
        float rightBound;// X-axis the enemy can move in

        bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        Vector2 lastPosition;
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        bool isPowered;
        int poweredTime = 500;

        string launcherId;
        public string LauncherId
        {
            get { return launcherId; }
        }

        #region movement
        public bool InTheAir
        {
            get { return canGoDown; }
        }
        public bool OnTheGround
        {
            get { return !canGoDown; }
        }

        bool canGoDown = true;
        public bool CanGoDown
        {
            get { return canGoDown; }
            set { canGoDown = value; }
        }
        bool canGoLeft = true;
        public bool CanGoLeft
        {
            get { return canGoLeft; }
            set { canGoLeft = value; }
        }
        bool canGoRight = true;
        public bool CanGoRight
        {
            get { return canGoRight; }
            set { canGoRight = value; }
        }
        #endregion

        /// <summary>
        /// Gets the sprite effects.
        /// </summary>
        SpriteEffects spriteEffects;
        public SpriteEffects SpriteEffects
        {
            get { return this.spriteEffects; }
        }

        public Enemy(Game1 game, Vector2 position, Vector2 speed, string launcherId)
            : base(game, position)
        {
            this.leftBound = 0;
            this.rightBound = Level.Width;
            if (speed != Vector2.Zero)
            {
                this.speed = speed;
                if (this.speed.X > 0)
                {
                    this.face_right = true;
                }
                else
                {
                    this.face_right = false;
                }
            
                isPowered = true;
            }
            this.launcherId = launcherId;
        }

        public Enemy(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string positiveStr = reader.GetAttribute("face_right");
            string leftStr = reader.GetAttribute("left");
            string rightStr = reader.GetAttribute("right");
            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            if (leftStr != null)
            {
                leftBound = float.Parse(leftStr);
            }
            else
            {
                leftBound = 0;
            }
            if (rightStr != null)
            {
                rightBound = float.Parse(rightStr);
            }
            else
            {
                rightBound = Level.Width;
            }
            this.game = game;
            this.launcherId = null;
            this.position = new Vector2(px, py);
            if (positiveStr == "true")
            {
                face_right = true;
            }
            else
            {
                face_right = false;
            }
        }

        public override void LoadContent()
        {
            runAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Enemy.Run"),
                DEFAULT_FRAME_SIZE, 100, true);
            dieAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Enemy.Die"),
                DEFAULT_FRAME_SIZE, 100, false);
            this.currentAnimation = runAnimation;
            isAlive = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.isAlive)
            {
                ApplyPhysics(gameTime);

                canGoDown = true;
                canGoLeft = true;
                canGoRight = true;

                CollisionDetection();

                if (canGoDown)
                {
                    lastPosition = position;
                }
            }
            else
            {
                dieTime += gameTime.ElapsedGameTime.Milliseconds;
                if (dieTime > DieTime)
                {
                   Level.MovableObjects.Remove(this);
                }
            }

            SetAnimation();
            base.Update(gameTime);
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            int elapsed = gameTime.ElapsedGameTime.Milliseconds;
            if (poweredTime > 0)
            {
                poweredTime -= elapsed;
            }
            else
            {
                isPowered = false;
            }

            // Base speed is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            if (!canGoLeft && speed.X < 0 ||
                !canGoRight && speed.X > 0)
            {
                face_right = !face_right;
            }

            if (!isPowered)
            {
                if (face_right)
                {
                    speed.X = horizontalMoveSpeed;
                }
                else
                {
                    speed.X = -horizontalMoveSpeed;
                }
            }

            speed.Y += (GravityAcceleration * elapsed) / 1000f;

            if (!canGoDown && speed.Y > 0)
            {
                speed.Y = 0;
            }

            speed.Y = MathHelper.Clamp(speed.Y, -MaxFallSpeed, MaxFallSpeed);

            if (!canGoDown && speed.Y > 0)
            {
                speed.Y = 0;
            }

            // out of bound
            if ((speed.X < 0 && position.X < leftBound + 5) ||
                (speed.X > 0 && position.X > rightBound - 50))
            {
                face_right = !face_right;
            }

            // out of border
            if ((speed.X < 0 && position.X < 3) ||
                (speed.X > 0 && position.X > Level.Width - 50) ||
                (speed.Y < 0 && position.Y < 3) ||
                (speed.Y > 0 && position.Y > Level.Height - 50))
            {
                Level.MovableObjects.Remove(this);
            }

            if (!canGoLeft && speed.X < 0 ||
                !canGoRight && speed.X > 0)
            {
                face_right = !face_right;
            }

            // Apply speed.
            position += speed;
        }

        private void SetAnimation()
        {
            if (!isAlive)
            {
                this.currentAnimation = dieAnimation;
            }
            else
            {
                this.currentAnimation = runAnimation;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (currentAnimation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Flip the sprite to face the way we are moving.
            if (speed.X > 0)
                spriteEffects = SpriteEffects.FlipHorizontally;
            else
                spriteEffects = SpriteEffects.None;

            // Draw the current frame.
            Game.SpriteBatch.Draw(currentAnimation.Texture, ScreenPosition,
                currentAnimation.CurrentSourceRectangle, Color.White,
                0.0f, currentAnimation.Origin, 1.0f, spriteEffects, 0.9f);
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player && this.isAlive)
            {
                Player player = (Player)obj;

                bool trample = false;

                if (player.Y + player.CollisionHeight < this.Y + this.CollisionHeight / 2)
                {
                    trample = true;
                }

                if (trample && player.IsAlive)
                {
                    this.isAlive = false;
                    player.IsJumping = true;
                    if (player.KillFirstEnemy)
                    {
                        player.KillsecondEnemy = true;
                    }
                    player.KillFirstEnemy = true;
                }
                else
                {
                    player.IsAlive = false;
                    player.KillFirstEnemy = false;
                }
            }
        }
    }
}
