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
    class Mirror : StaticObject, IControlledObject, IController
    {
        public override int CollisionWidth
        {
            get { return 90; }
        }
        public override int CollisionHeight
        {
            get { return 90; }
        }

        KeyUpHint keyUpHint;

        /// <summary>
        /// The angle between the normal and the x-axis
        /// </summary>
        public float NormalAngle
        {
            get { return normalAngle; }
            set { normalAngle = value; }
        }
        float normalAngle = 0;

        private const float RotationSpeed = 0.01f;

        public Mirror(Game1 game, Vector2 pos)
            : base(game, pos)
        {
            position = pos;
        }

        public Mirror(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr2 = reader.GetAttribute("px");
            string pyStr2 = reader.GetAttribute("py");
            string normalAngleStr = reader.GetAttribute("normalAngle");

            float px2 = float.Parse(pxStr2);
            float py2 = float.Parse(pyStr2);
            if (normalAngleStr != null)
                this.normalAngle = float.Parse(normalAngleStr);

            this.game = game;
            this.position = new Vector2(px2, py2);
            keyUpHint = new KeyUpHint(game, new Vector2(px2 + 5, py2 - 15), this.id + "hint");
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Mirror.Mirror1");
        }

        public override void Update(GameTime gameTime)
        {
            keyUpHint.Position = new Vector2(position.X + 5, position.Y - 15);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(texture,
                new Vector2((int)ScreenPosition.X + HalfWidth, (int)ScreenPosition.Y + HalfHeight),
                null, Color.White, normalAngle,
                new Vector2(HalfWidth, HalfHeight),
                scale, SpriteEffects.None, layerDepth);
        }

        public override void CollisionResponse(Object obj)
        {
            if (obj is Player)
            {
                ((IControlledObject)this).Control(this);
                this.keyUpHint.IsDisplay = true;
            }
        }

        #region IControlledObject Members

        void IControlledObject.Control(IController controller)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                this.normalAngle += RotationSpeed;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.normalAngle -= RotationSpeed;
            }
        }

        #endregion

        #region IController Members

        void IController.Add(IControlledObject obj)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
