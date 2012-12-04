using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public class Door : StaticObject
    {
        public override int CollisionWidth
        {
            get { return 51; }
        }
        public override int CollisionHeight
        {
            get { return 89; }
        }
        string LevelFileName
        {
            get { return levelFileName; }
        }
        string levelFileName;

        string levelName;

        KeyUpHint keyUpHint;

        string bindingId;


        public Door(Game1 game, Vector2 position, string levelFileName)
            : base(game, position)
        {
            this.levelFileName = levelFileName;
            this.keyUpHint = new KeyUpHint(game, new Vector2(position.X + 7, position.Y - 35),
                "Door_hint");
        }

        public Door(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            this.levelFileName = reader.GetAttribute("levelFileName");
            this.levelName = reader.GetAttribute("levelName");
            this.bindingId = reader.GetAttribute("bindingId");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);

            this.game = game;
            this.position = new Vector2(px, py);
            this.keyUpHint = new KeyUpHint(game, new Vector2(px + 7, py - 35),
                this.id + "hint");
        }

        public override void LoadContent()
        {
            this.texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Door.Door2");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                Level.Player.CollidedWithDoor = true;
                Level.Player.ComingLevel = levelFileName;
                this.keyUpHint.IsDisplay = true;
                Level.Player.BornPointId = this.bindingId;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Vector2 screenPos = ScreenPosition;
            Game.SpriteBatch.DrawString(Game.font, levelName,
                new Vector2(screenPos.X - 10, screenPos.Y - 35), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
        }
    }
}
