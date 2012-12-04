using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demeter
{
    public class LightRay : StaticObject, IController
    {
        public override int CollisionWidth
        {
            get { return 0; }
        }
        public override int CollisionHeight
        {
            get { return 0; }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        float angle;

        LightSource lightSource;
        public LightSource LightSource
        {
            get { return lightSource; }
            set { lightSource = value; }
        }

        List<Vector2> reflectionPosition = new List<Vector2>();

        const int RayElementWidth = 10;

        public LightRay(Game1 game, Vector2 position, float angle)
            : base(game, position)
        {
            this.angle = angle;
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.StaticObject.LadderRay.LadderRay1");
            /*texture = new Texture2D(Game.GraphicsDevice, RayElementWidth, RayElementWidth);

            int count = RayElementWidth * RayElementWidth;
            Color[] color = new Color[count];

            for (int i = 0; i < count; i++)
                color[i] = new Color(200, 200, 200, 120);
             texture.SetData(color);*/
        }

        public override void Update(GameTime gameTime)
        {
            angle = Helper.Normalize(lightSource.Angle);

            reflectionPosition.Clear();
            reflectionPosition.Add(position);

            float incidenceAngle = angle;

            Vector2? collidingPosition;
            Object obj = Level.FindObject(position, angle, out collidingPosition, lightSource);
            while (obj != null)
            {
                if (obj is Mirror
                    && Math.Abs(Helper.Normalize(((Mirror)obj).NormalAngle) - incidenceAngle) > Math.PI / 2)
                {
                    Mirror mirror = (Mirror)obj;

                    reflectionPosition.Add(collidingPosition.Value);

                    float reflectAngle = 2 * mirror.NormalAngle - incidenceAngle - (float)Math.PI;
                    obj = Level.FindObject(collidingPosition.Value, reflectAngle,
                        out collidingPosition, mirror);

                    incidenceAngle = Helper.Normalize(reflectAngle);
                }
                else
                {
                    reflectionPosition.Add(collidingPosition.Value);
                    if (obj is Gate)
                    {
                        ((IControlledObject)obj).Control(this);
                    }
                    break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            List<Vector2>.Enumerator currentEnum = reflectionPosition.GetEnumerator();
            List<Vector2>.Enumerator nextEnum = reflectionPosition.GetEnumerator();
            nextEnum.MoveNext();
            while(nextEnum.MoveNext() && currentEnum.MoveNext())
            {
                Vector2 current = currentEnum.Current;
                Vector2 next = nextEnum.Current;
                Vector2 currentPoint = Level.ScreenPosition(current);
                Vector2 nextPoint = Level.ScreenPosition(next);
                LineSegment lineSegment = new LineSegment(currentPoint, nextPoint);
                lineSegment.Retrieve(10, DrawPoint);
            }
        }

        public void DrawPoint(Vector2 point, float angle)
        {
            Game.SpriteBatch.Draw(texture, point,
                null, Color.White, angle, new Vector2((float)RayElementWidth / 2), 1, SpriteEffects.None, layerDepth);
        }

        public override void CollisionResponse(Object obj)
        {
        }

        #region IController Members

        void IController.Add(IControlledObject obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
