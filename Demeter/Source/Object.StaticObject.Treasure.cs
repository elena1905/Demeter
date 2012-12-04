using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    class Treasure : StaticObject
    {
        #region logical
        public override int CollisionWidth
        {
            get { return 50; }
        }

        public override int CollisionHeight
        {
            get { return 50; }
        }
        #endregion


        bool isGotten;
        Texture2D textureOn;
        Texture2D textureOff;
        int time = 0;
        const int IntervelTime = 100;
        readonly int top;
        readonly int bottom;
        bool positive = false;

        public Treasure(Game1 game,string id, bool isGotten)
            : base(game)
        {
            this.id = id;
            this.isGotten = isGotten;
            LoadContent();
            top = (int)Y - 20;
            bottom = (int)Y;
        }

        public Treasure(Game1 game, XmlTextReader reader, bool isGotten)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            this.id = reader.GetAttribute("id");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            this.position = new Vector2(px, py);

            LoadContent();

            this.isGotten = isGotten;
            if (isGotten)
            {
                this.Texture = textureOff;
            }
            else
            {
                this.Texture = textureOn;
            }

            top = (int)Y - 20;
            bottom = (int)Y;
        }

        public override void LoadContent()
        {
            if (id == "treasure1")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Red");
            else if (id == "treasure2")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Orange");
            else if (id == "treasure3")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Yellow");
            else if (id == "treasure4")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Green");
            else if (id == "treasure5")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Cyan");
            else if (id == "treasure6")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Blue");
            else if (id == "treasure7")
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Purple");
            else
                textureOn = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.Red");

            textureOff = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Treasure.LightOff");
        }
        
        public override void Update(GameTime gameTime)
        {
            if (!isGotten)
            {
                time += gameTime.ElapsedGameTime.Milliseconds;
                if (time > IntervelTime)
                {
                    time -= IntervelTime;

                    Y += positive ? 5 : -5;

                    if (Y < top)
                        positive = true;
                    else if (Y > bottom)
                        positive = false;
                }
            }
        }

        public override void CollisionResponse(Object obj)
        {
            if (!isGotten)
            {
                if (obj is Player && ((Player)obj).IsAlive)
                {
                    isGotten = true;
                    Level.TreasureMgr.GetTreasure(Level.LevelFileName.Substring(0, 6),
                        Level.LevelFileName, this.id);
                    this.Texture = textureOff;
                    Level.GottenTreasure++;
                }
            }
        }
    }
}
