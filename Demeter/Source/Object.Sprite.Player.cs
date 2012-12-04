using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace Demeter
{
    public class Player : Sprite
    {
        #region Logical
        public override int CollisionWidth
        {
            get { return 45; }
        }
        public override int CollisionHeight
        {
            get { return 70; }
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

        static readonly Point DEFAULT_FRAME_SIZE = new Point(45, 70);

        // animations
        Animation idleAnimation;
        Animation runAnimation;
        Animation jumpAnimation;
        Animation dieAnimation;
        Animation climbAnimation;

        #endregion

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 150.0f;
        private const float MaxMoveSpeed = 8.0f;

        // Constants for controlling vertical movement
        private const float GravityAcceleration = 25.0f;
        private const float MaxFallSpeed = 12.0f;
        private const float jumpStartSpeed = -8f;
        private const float speedOnLadder = 2f;
        float addSpeed = -4;

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        Vector2 speed = Vector2.Zero;

        public bool FacingRight
        {
            get { return facingRight; }
        }
        bool facingRight = true;

        /// <summary>
        /// Gets whether or not the player is alive
        /// </summary>
        bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        private bool isJumping;
        public bool IsJumping
        {
            get { return isJumping; }
            set { isJumping = value; }
        }

        public bool IsLeaving
        {
            get { return isLeaving; }
            set { isLeaving = value; }
        }
        private bool isLeaving;

        public string ComingLevel
        {
            get { return comingLevel; }
            set { comingLevel = value; }
        }
        string comingLevel;

        public bool InTheAir
        {
            get { return canGoDown; }
        }
        public bool OnTheGround
        {
            get { return !canGoDown; }
        }
	
        Vector2 lastPosition;
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        string bornPointId;
        public string BornPointId
        {
            get { return bornPointId; }
            set { bornPointId = value; }
        }


        #region two_step jumping
        bool killFirstEnemy;
        public bool KillFirstEnemy
        {
            get { return killFirstEnemy; }
            set { killFirstEnemy = value; }
        }

        bool killsecondEnemy;
        public bool KillsecondEnemy
        {
            get { return killsecondEnemy; }
            set { killsecondEnemy = value; }
        }
        #endregion

        #region movement

        bool tryClimbing = false;

        bool canGoUp = true;
        public bool CanGoUp
        {
            get { return canGoUp; }
            set { canGoUp = value; }
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
        private bool collidedWithLadder;
        public bool CollidedWithLadder
        {
            get { return collidedWithLadder; }
            set { collidedWithLadder = value; }
        }

        private bool collidedWithDoor;
        public bool CollidedWithDoor
        {
            get { return collidedWithDoor; }
            set { collidedWithDoor = value; }
        }
        #endregion

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private int horizontalMovement = 0;
        private int verticalMovement = 0;
        private bool isLadderUsed = false;
        private Ladder ladderUsed;
        internal Ladder LadderUsed
        {
            set { ladderUsed = value; }
        }

        /// <summary>
        /// Gets the sprite effects.
        /// </summary>
        SpriteEffects spriteEffects;
        public SpriteEffects SpriteEffects
        {
            get { return this.spriteEffects; }
        }

        public Player(Game1 game, Vector2 position)
            : base(game, position)
        {
        }

        public Player(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            this.game = game;

            // born
            /*
            if (Game.PlayerBornPointId != null)
            {
                this.position = Game.BindingPoint.GetPosById(Game.Current_levelFileName, Game.PlayerBornPointId);
            }
                // reborn*/
  
            if (Game.BindingPoint != null)
            {
                this.position = Game.BindingPoint.JudgeBindingPoint(Game.Current_levelFileName, Game.DiePosition);
            }
            else
            {
                this.position = new Vector2(px, py);
            }
        }

        /// <summary>
        /// Load resources
        /// </summary>
        public override void LoadContent()
        {
            idleAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Player.Idle"),
                DEFAULT_FRAME_SIZE, 100, true);
            runAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Player.Run"),
                DEFAULT_FRAME_SIZE, 100, true);
            jumpAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Player.Jump"),
                DEFAULT_FRAME_SIZE, 100, false);
            dieAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Player.Die"),
                DEFAULT_FRAME_SIZE, 100, false);
            climbAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Player.Climb"),
                DEFAULT_FRAME_SIZE, 100, true);


            this.currentAnimation = idleAnimation;
            isAlive = true;
        }


        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (this.IsAlive)
            {
                GetInput();

                if (!canGoDown)
                {
                    killFirstEnemy = false;
                    killsecondEnemy = false;
                }
                ApplyPhysics(gameTime);
            }

            SetAnimation();

            // Clear input.
            horizontalMovement = 0;
            verticalMovement = 0;
            canGoUp = true;
            canGoDown = true;
            canGoLeft = true;
            canGoRight = true;
            collidedWithLadder = false;
            collidedWithDoor = false;
            isJumping = false;

            CollisionDetection();

            if (canGoDown)
            {
                lastPosition = position;
            }

            if (Game.BindingPoint != null)
            {
                Game.BindingPoint.PassBindingPoint(Level.LevelFileName, this.position);
            }

            if (!(this.currentAnimation == climbAnimation && !tryClimbing))
            {
                base.Update(gameTime);
            }

            tryClimbing = false;
        }

        /// <summary>
        /// Get input from the keyboard.
        /// If there is any controll-key pressed, return true.
        /// Otherwise, return false.
        /// </summary>
        public void GetInput()
        {
            bool tryJumping = false;
            bool tryLeaving = false;
            // Get input state.
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {   // player moves left
                 horizontalMovement -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {   // player moves right
                horizontalMovement += 1;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {   //player moves up
                verticalMovement -= 1;
                tryClimbing = true;
                tryLeaving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {   //player moves down
                verticalMovement += 1;
                tryClimbing = true;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {   // player jump
                tryJumping = true;
            }

            if (tryJumping && OnTheGround && canGoUp)
            {
                isJumping = true;
            }
            if (verticalMovement != 0 && !isLadderUsed && collidedWithLadder && this.Y >= ladderUsed.Y)
            {
                float offset = (ladderUsed.X + ladderUsed.CollisionWidth / 2) -
                    (this.X + CollisionWidth / 2);
                if (Math.Abs(offset) < 5f)
                {
                    this.X += offset;
                    isLadderUsed = true;
                }
                else if (horizontalMovement == 0)
                {
                    horizontalMovement = offset >= 0 ? 1 : -1;
                }
            }
            if (!collidedWithLadder || tryJumping ||
                (horizontalMovement != 0 && (verticalMovement == 0 || this.Y < ladderUsed.Y)))
            {
                isLadderUsed = false;
            }
            if (!isLadderUsed)
            {
                verticalMovement = 0;
            }
            if (collidedWithDoor && tryLeaving)
            {
                isLeaving = true;
            }

            if (horizontalMovement == 1)
            {
                facingRight = true;
            }
            else if (horizontalMovement == -1)
            {
                facingRight = false;
            }
        }

        /// <summary>
        /// Updates the player's speed and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            int elapsed = gameTime.ElapsedGameTime.Milliseconds;

            // Base speed is a combination of horizontal movement control and
            // acceleration downward due to gravity.

            if (isLadderUsed)
            {
                speed.X = 0;
                if (position.Y < ladderUsed.Y && verticalMovement == -1)
                {
                    speed.Y = 0;
                }
                else
                {
                    speed.Y = verticalMovement * speedOnLadder;
                }
                if (verticalMovement == -1 && position.Y + CollisionHeight < ladderUsed.Y)
                {
                    speed.Y = 0;
                }
            }
            else
            {
                if (InTheAir)
                {
                    if (horizontalMovement != 0)
                        speed.X = horizontalMovement * MoveAcceleration * elapsed / 1000f;
                }
                else
                {
                    speed.X = horizontalMovement * MoveAcceleration * elapsed / 1000f;
                }
                if (isJumping)
                {
                    speed.Y = jumpStartSpeed;
                    if (killsecondEnemy)
                    {
                        speed.Y += addSpeed;
                    }
                }
                else
                {
                    speed.Y += (GravityAcceleration * elapsed) / 1000f;
                }
            }

            // Prevent the player from running faster than his top speed.
            speed.X = MathHelper.Clamp(speed.X, -MaxMoveSpeed, MaxMoveSpeed);
            speed.Y = MathHelper.Clamp(speed.Y, -MaxFallSpeed, MaxFallSpeed);

            if (!canGoLeft && speed.X < 0 ||
                !canGoRight && speed.X > 0)
            {
                speed.X = 0;
            }
            if (!canGoUp && speed.Y < 0 ||
                !canGoDown && speed.Y > 0)
            {
                speed.Y = 0;
            }

            // Out of Border
            if ((speed.X < 0 && position.X < 3) ||
                (speed.X > 0 && position.X > Level.Width - 50))
            {
                speed.X = 0;
            }
            if ((speed.Y < 0 && position.Y < 3) ||
                (speed.Y > 0 && position.Y > Level.Height - 50))
            {
                speed.Y = 0;
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
            else if (speed.Y != 0 && !isLadderUsed)
            {
                this.currentAnimation = jumpAnimation;
            }
            else if (isLadderUsed)
            {
                this.currentAnimation = climbAnimation;
            }
            else if (speed.X != 0)
            {
                this.currentAnimation = runAnimation;
            }
            else
            {
                this.currentAnimation = idleAnimation;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (currentAnimation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Flip the sprite to face the way we are moving.
            if (facingRight)
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
        }
    }
}
