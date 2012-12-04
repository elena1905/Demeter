using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public abstract class Sprite : Object
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        protected Animation currentAnimation;
        public Animation CurrentAnimation
        {
            get { return this.currentAnimation; }
        }

        /// <summary>
        /// The amount of time in milliseconds that the current frame has been shown for.
        /// </summary>
        protected float time;

        // time used to playing the die animation
        public int DieTime
        {
            get { return 1000; }
        }

        /// <summary>
        /// Gets the collision rectangle.
        /// </summary>
        public override Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)(position.X),
                    (int)(position.Y), CollisionWidth, CollisionHeight);
            }
        }

        public Sprite(Game1 game)
            : base(game)
        {
        }

        public Sprite(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
        }

        public Sprite(Game1 game, Vector2 position)
            : base(game, position)
        {
        }

        public Sprite(Game1 game, Vector2 position,
            int collisionOffset, float scale)
            : base(game, position, scale)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Process passing time.
            currentAnimation.Update(gameTime);
        }
    }
}
