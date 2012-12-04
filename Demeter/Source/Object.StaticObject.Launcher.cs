using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Demeter
{
    class Launcher : StaticObject
    {
        #region logical
        public override int CollisionWidth
        {
            get { return 70; }
        }

        public override int CollisionHeight
        {
            get { return 50; }
        }
        #endregion

        //generate things
        int passingTime = 0;
        int generateTime;
        string generateType;
        int generateAmount;

        Vector2 powerSpeed;
        SpriteEffects spriteEffects;

        public Launcher(Game1 game, Vector2 position)
            : base(game, position)
        {
        }

        public Launcher(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string generateTimeStr = reader.GetAttribute("generate_time");
            string generateAmountStr = reader.GetAttribute("generateAmount");
            string speedXStr = reader.GetAttribute("speed_x");
            string speedYStr = reader.GetAttribute("speed_y");
            string direction = reader.GetAttribute("direction");

            this.generateType = reader.GetAttribute("generate_type");
            this.generateTime = int.Parse(generateTimeStr);
            this.game = game;

            if (direction == "right")
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                spriteEffects = SpriteEffects.None;
            }
            if (generateAmountStr == null)
            {
                this.generateAmount = -1;
            }
            else
            {
                this.generateAmount = int.Parse(generateAmountStr);
            }

            float speedX = float.Parse(speedXStr);
            float speedY = float.Parse(speedYStr);
            this.powerSpeed = new Vector2(speedX, speedY);

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            this.position = new Vector2(px, py);
        }
        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Launcher"); ;
        }

        public override void Update(GameTime gameTime)
        {
            passingTime += gameTime.ElapsedGameTime.Milliseconds;

            if (passingTime > generateTime)
            {
                passingTime -= generateTime;
                if (generateAmount == -1)
                {
                    Generate();
                }
                else
                {
                    int existAmount = 0;
                    foreach (Object obj in Level.MovableObjects)
                    {
                        if (obj is Enemy && ((Enemy)obj).LauncherId == this.Id) 
                        {
                            existAmount++;
                        }
                    }
                    if (existAmount < generateAmount)
                    {
                        Generate();
                    }
                }
            }
        }

        public override void CollisionResponse(Object obj)
        {
            Location location = LocationOf(obj);

            if (obj is Player)
            {
                Player player = (Player)obj;

                switch (location)
                {
                    case Location.BELOW:
                        obj.Y = this.Y + this.CollisionHeight - 1;
                        player.CanGoUp = false;
                        break;
                    case Location.ABOVE:
                        obj.Y = this.Y - obj.CollisionHeight + 1;
                        player.CanGoDown = false;
                        break;
                    case Location.RIGHT:
                        obj.X = this.X + this.CollisionWidth - 1;
                        player.CanGoLeft = false;
                        break;
                    case Location.LEFT:
                        obj.X = this.X - obj.CollisionWidth + 1;
                        player.CanGoRight = false;
                        break;
                }
            }
        }

        public void Generate()
        {
            if (generateType == "enemy")
            {
                Enemy enemy = new Enemy(game, this.position, powerSpeed ,this.Id);
                Level.MovableObjects.Add(enemy);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(texture, ScreenRectangle, null,
                Color.White, 0, Vector2.Zero, spriteEffects, layerDepth);
        }
    }
}
