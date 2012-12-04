using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace Demeter
{
    class Switch : StaticObject, IController
    {
        public override int CollisionWidth
        {
            get { return 45; }
        }
        public override int CollisionHeight
        {
            get { return 70; }
        }

        bool switchOn;

        List<IControlledObject> controlled = new List<IControlledObject>();

        Texture2D switchOnTexture;
        Texture2D switchOffTexture;

        KeyUpHint keyUpHint;

        private bool wasKeyDown = false;

        public Switch(Game1 game, Vector2 pos)
            : base(game, pos)
        {
            position = pos;
            this.keyUpHint = new KeyUpHint(game, new Vector2(position.X + 7, position.Y - 35),
                "Switch_hint");
        }

        public Switch(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);

            this.game = game;
            this.position = new Vector2(px, py);
            this.keyUpHint = new KeyUpHint(game, new Vector2(px + 7, py - 35),
                this.id + "hint");
        }

        public override void LoadContent()
        {
            switchOnTexture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Switch.SwitchOn");
            switchOffTexture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Switch.SwitchOff");
            texture = switchOffTexture;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public override void Update(GameTime gameTime)
        {
            keyUpHint.Position = new Vector2(position.X + 7, position.Y - 35);
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    if (!wasKeyDown)
                    {
                        foreach (IControlledObject controlledObj in controlled)
                        {
                            controlledObj.Control(this);
                        }
                        switchOn = !switchOn;
                    }
                    wasKeyDown = true;
                }
                else
                {
                    wasKeyDown = false;
                }

                if (switchOn)
                    texture = switchOnTexture;
                else
                    texture = switchOffTexture;

                this.keyUpHint.IsDisplay = true;
            }
        }

        #region IController Members

        void IController.Add(IControlledObject obj)
        {
            this.controlled.Add(obj);
        }

        #endregion
    }
}
