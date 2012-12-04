using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public class Platform : Tile
    {

        public Platform(Game1 game, Vector2 position, Point tileFrame)
            : base(game, position, tileFrame)
        {
        }

        public Platform(Game1 game, Vector2 position, int width)
            : base(game, position, width)
        {
        }

        public Platform(Game1 game, Vector2 position, int width, int height)
            : base(game, position, width, height)
        {
        }

        public Platform(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string widthStr = reader.GetAttribute("width");
            string heightStr = reader.GetAttribute("height");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            int width = int.Parse(widthStr);
            int height = int.Parse(heightStr);

            this.position = new Vector2(px, py);
            this.Width = width;
            this.Height = height;
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Platform.Platform1");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                Player player = (Player)obj;

                if ((int)player.LastPosition.Y + player.CollisionHeight <= this.Y)
                {
                    player.CanGoDown = false;
                    player.Y = this.Y - player.CollisionHeight + 1;
                }
            }
            else if (obj is Enemy)
            {
                Enemy enemy = (Enemy)obj;

                if ((int)enemy.LastPosition.Y + enemy.CollisionHeight <= this.Y)
                {
                    enemy.CanGoDown = false;
                    enemy.Y = this.Y - enemy.CollisionHeight + 1;
                }
            }
        }
    }
}
