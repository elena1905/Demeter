using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Demeter
{
    public class OgreBase : StaticObject
    {
        public override int CollisionWidth
        {
            get { return 81; }
        }

        public override int CollisionHeight
        {
            get { return 50; }
        }

        public OgreBase(Game1 game, Vector2 position)
            : base(game)
        {
            this.game = game;
            this.position = position;

            Level.Objects.Add(this);
            LoadContent();
        }
        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Ogre.Base");
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void CollisionResponse(Object obj)
        {
            Location location = LocationOf(obj);

            switch (location)
            {
                case Location.BELOW:
                    obj.Y = this.Y + this.CollisionHeight - 1;
                    break;
                case Location.ABOVE:
                    obj.Y = this.Y - obj.CollisionHeight + 1;
                    break;
                case Location.RIGHT:
                    obj.X = this.X + this.CollisionWidth - 1;
                    break;
                case Location.LEFT:
                    obj.X = this.X - obj.CollisionWidth + 1;
                    break;
            }

            if (obj is Player)
            {
                Player player = (Player)obj;
                switch (location)
                {
                    case Location.BELOW:
                        player.CanGoUp = false;
                        break;
                    case Location.ABOVE:
                        player.CanGoDown = false;
                        break;
                    case Location.RIGHT:
                        player.CanGoLeft = false;
                        break;
                    case Location.LEFT:
                        player.CanGoRight = false;
                        break;
                }
            }
            else if (obj is Enemy)
            {
                Enemy enemy = (Enemy)obj;
                switch (location)
                {
                    case Location.ABOVE:
                        enemy.CanGoDown = false;
                        break;
                    case Location.RIGHT:
                        enemy.CanGoLeft = false;
                        break;
                    case Location.LEFT:
                        enemy.CanGoRight = false;
                        break;
                }
            }
        }
    }
}
