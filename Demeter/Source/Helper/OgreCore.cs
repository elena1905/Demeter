using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Demeter
{
    public class OgreCore : Sprite
    {
        public override int CollisionWidth
        {
            get { return 45; }
        }

        public override int CollisionHeight
        {
            get { return 70; }
        }

        public override int TopCollisionOffset
        {
            get { return 0; }
        }

        public override int LeftCollisionOffset
        {
            get { return 0; }
        }


        Animation animation;
        static readonly Point DEFAULT_FRAME_SIZE = new Point(45, 70);


        float maxOffset;
        float offset = 0;
        int bottomTime = 0;
        int topTime = 0;
        const float verticalSpeed = -3.5f;
        bool goUp = true;
        bool move = true;

        public OgreCore(Game1 game, Vector2 position, float Maxoffset)
            : base(game)
        {
            this.game = game;
            this.position = new Vector2(position.X + 18, position.Y + 13);
            this.maxOffset = Maxoffset;

            Level.MovableObjects.Add(this);
            LoadContent();
        }

        public override void LoadContent()
        {
            this.animation = new Animation(Game.Content.Load<Texture2D>("texture/Object.StaticObject.Ogre.Core"),
                DEFAULT_FRAME_SIZE, 100, true);
            this.currentAnimation = animation;
        }

        public override void Update(GameTime gameTime)
        {
            if (move == false && offset == 0)
            {
                bottomTime += gameTime.ElapsedGameTime.Milliseconds;
                if (bottomTime > 2000)
                {
                    move = true;
                    bottomTime = 0;
                }
            }
            else if (move == false && offset == maxOffset)
            {
                topTime += gameTime.ElapsedGameTime.Milliseconds;
                if (topTime > 500)
                {
                    move = true;
                    topTime = 0;
                }
            }
            if (offset > 0)
            {
                goUp = !goUp;
                offset = 0;
                move = false;
            }
            else if (offset < maxOffset)
            {
                goUp = !goUp;
                offset = maxOffset;
                move = false;
            }

            if (goUp && move)
            {
                position.Y += verticalSpeed;
                offset += verticalSpeed;
            }
            else if (!goUp && move)
            {
                position.Y -= verticalSpeed;
                offset -= verticalSpeed;
            }

            base.Update(gameTime);
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                ((Player)obj).IsAlive = false;
            }
            else if (obj is Enemy)
            {
                ((Enemy)obj).IsAlive = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (currentAnimation == null)
                throw new NotSupportedException("No animation is currently playing.");


            // Draw the current frame.
            Game.SpriteBatch.Draw(currentAnimation.Texture, ScreenPosition,
                currentAnimation.CurrentSourceRectangle, Color.White,
                0.0f, currentAnimation.Origin, 1.0f, SpriteEffects.None, 0.3f);
        }
    }
}
