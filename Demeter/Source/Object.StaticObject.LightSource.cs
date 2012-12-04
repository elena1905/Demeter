using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace Demeter
{
    public class LightSource : StaticObject, IControlledObject
    {
        public override int CollisionWidth
        {
            get { return 65; }
        }
        public override int CollisionHeight
        {
            get { return 43; }
        }
        /// <summary>
        /// The angle between the normal and the x-axis
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        float angle = 0;

        bool switchOn = true;
        public bool SwitchOn
        { 
            get { return switchOn; }
        }

        private const float RotationSpeed = 0.005f;

        LightRay lightRay;

        public LightSource(Game1 game, Vector2 pos)
            : base(game, pos)
        {
            this.lightRay = new LightRay(game, Helper.ToVector2(CollisionRect.Center), angle);
            this.lightRay.LightSource = this;
        }

        public LightSource(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr2 = reader.GetAttribute("px");
            string pyStr2 = reader.GetAttribute("py");
            string angleStr = reader.GetAttribute("angle");
            string switchOnStr = reader.GetAttribute("switchOn");

            float px2 = float.Parse(pxStr2);
            float py2 = float.Parse(pyStr2);
            if (angleStr != null)
                this.angle = float.Parse(angleStr);
            if (switchOnStr != null && switchOnStr == "false")
                this.switchOn = false;

            this.game = game;
            this.position = new Vector2(px2, py2);
            this.lightRay = new LightRay(game, Helper.ToVector2(CollisionRect.Center), angle);
            this.lightRay.LightSource = this;
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.Light.Light1");
        }

        public override void Update(GameTime gameTime)
        {
            if(switchOn)
                lightRay.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(texture,
                new Vector2((int)ScreenPosition.X + HalfWidth, (int)ScreenPosition.Y + HalfHeight),
                null, Color.White, angle,
                new Vector2(HalfWidth, HalfHeight),
                scale, SpriteEffects.None, 1);
            if(switchOn)
                lightRay.Draw(gameTime);
        }

        public override void CollisionResponse(Object obj)
        {
            //throw new NotImplementedException();
        }

        #region IControlledObject Members

        void IControlledObject.Control(IController controller)
        {
            if (controller is ShiftStick)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    this.angle += RotationSpeed;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    this.angle -= RotationSpeed;
                }
            }
            else if (controller is Switch)
            {
                switchOn = !switchOn;
            }
        }

        #endregion
    }
}
