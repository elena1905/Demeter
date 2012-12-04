using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    class Ladder : StaticObject
    {
        public override int CollisionWidth
        {
            get { return 45; }
        }

        public override int CollisionHeight
        {
            get { return height; }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                frame = (int)Math.Ceiling((double)height / texture.Height);
            }
        }

        private int frame;

        public Ladder(Game1 game, Vector2 position, int height)
            :base(game, position)
        {
            this.Height = height;
        }

        public Ladder(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string heightStr = reader.GetAttribute("height");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            int height = int.Parse(heightStr);

            this.position = new Vector2(px, py);
            this.Height = height;
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Ladder.Ladder1");
        }

        public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < frame; i++)
            {
                Rectangle screenRect = new Rectangle((int)ScreenPosition.X,
                    (int)ScreenPosition.Y + i * texture.Height, texture.Width, texture.Height);
                Game.SpriteBatch.Draw(texture, screenRect, null, Color.White,
                    0, Vector2.Zero, SpriteEffects.None, layerDepth);
            }
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                Level.Player.LadderUsed = this;
                Level.Player.CollidedWithLadder = true;
            }
        }
    }
}
