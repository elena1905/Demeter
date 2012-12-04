using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demeter
{
    public class KeyUpHint : Sprite
    {
        public override int CollisionWidth
        {
            get { return 0; }
        }
        public override int CollisionHeight
        {
            get { return 0; }
        }

        Animation displayAnimation;
        static readonly Point DEFAULT_FRAME_SIZE = new Point(32, 35);

        bool isDisplay;
        public bool IsDisplay
        {
            get { return isDisplay; }
            set { isDisplay = value; }
        }

        /// <summary>
        /// Gets the sprite effects.
        /// </summary>
        SpriteEffects spriteEffects;
        public SpriteEffects SpriteEffects
        {
            get { return this.spriteEffects; }
        }

        public KeyUpHint(Game1 game, Vector2 position, string id)
            : base(game)
        {
            this.id = id;
            this.game = game;
            this.position = position;
            Level.Objects.Add(this);
        }

        public override void LoadContent()
        {
            displayAnimation = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Hint.DoorHint"),
                DEFAULT_FRAME_SIZE, 200, true);
            this.currentAnimation = displayAnimation;
            isDisplay = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void CollisionResponse(Object obj)
        { }

        public override void Draw(GameTime gameTime)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
            if (isDisplay)
            {
                Game.SpriteBatch.Draw(currentAnimation.Texture, ScreenPosition,
                 currentAnimation.CurrentSourceRectangle, Color.White,
                 0.0f, currentAnimation.Origin, 1.0f, spriteEffects, 0.9f);
            }
            isDisplay = false;
        }
    }
}
