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
    class ShiftStick : StaticObject, IController
    {
        public override int CollisionWidth
        {
            get { return 45; }
        }
        public override int CollisionHeight
        {
            get { return 70; }
        }

        List<IControlledObject> controlled = new List<IControlledObject>();

        Texture2D positiveTexture;
        Texture2D negativeTexture;
        Texture2D switchOffTexture;

        KeyUpHint keyUpHint;

        public ShiftStick(Game1 game, Vector2 pos, bool one_off, bool moveable)
            : base(game, pos)
        {
            position = pos;
            this.keyUpHint = new KeyUpHint(game, new Vector2(position.X + 7, position.Y - 35),
                "ShiftStick_hint");
        }

        public ShiftStick(Game1 game, XmlTextReader reader)
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
            positiveTexture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.ShiftStick.Positive");
            negativeTexture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.ShiftStick.Negative");
            switchOffTexture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.ShiftStick.SwitchOff");
            texture = switchOffTexture;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    foreach (IControlledObject controlledObj in controlled)
                    {
                        controlledObj.Control(this);
                    }
                    texture = positiveTexture;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    foreach (IControlledObject controlledObj in controlled)
                    {
                        controlledObj.Control(this);
                    }
                    texture = negativeTexture;
                }
                else
                {
                    texture = switchOffTexture;
                }
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
