using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml;

namespace Demeter
{
    public abstract class Object
    {
        protected string id;
        public string Id
        {
            get { return id; }
        }

        protected Game1 game;
        public Game1 Game
        {
            get { return this.game; }
        }

        public Level Level
        {
            get { return this.Game.Level; }
        }

        protected Vector2 position = Vector2.Zero;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Top
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public float Bottom
        {
            get { return position.Y + CollisionHeight; }
            set { position.X = value - CollisionHeight; }
        }
        public float Left
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float Right
        {
            get { return position.X + CollisionWidth; }
            set { position.X = value - CollisionWidth; }
        }

        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(position.X - Level.CameraOffset.X + TopCollisionOffset,
                    position.Y - Level.CameraOffset.Y + LeftCollisionOffset);
            }
        }

        protected bool movable;
        public bool Movable
        {
            get { return movable; }
            set { movable = value; }
        }

        protected float layerDepth = 0.6f;

        protected float scale = 1.0f;

        public virtual int TopCollisionOffset
        {
            get { return 0; }
        }
        public virtual int LeftCollisionOffset
        {
            get { return 0; }
        }

        public virtual int CollisionWidth
        {
            get { return 0; }
        }
        public virtual int CollisionHeight
        {
            get { return 0; }
        }
        public abstract Rectangle CollisionRect { get; }

        public Object(Game1 game)
        {
            this.game = game;
            LoadContent();
        }

        public Object(Game1 game, XmlTextReader reader)
            : this(game)
        {
            this.id = reader.GetAttribute("id");
            string movable = reader.GetAttribute("movable");
            if (movable != null && movable == "true")
            {
                this.movable = true;
                Level.MovableObjects.Add(this);
            }
            else
            {
                this.movable = false;
                Level.Objects.Add(this);
            }
        }

        public Object(Game1 game, Vector2 position)
            : this(game)
        {
            this.position = position;
        }

        public Object(Game1 game, Vector2 position, float scale)
            : this(game)
        {
            this.position = position;
            this.scale = scale;
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public abstract void CollisionResponse(Object obj);

        protected virtual void CollisionDetection()
        {
            List<Object> collided = Level.CollidedWith(this);
            foreach (Object obj in collided)
            {
                this.CollisionResponse(obj);
                obj.CollisionResponse(this);
            }
        }

        protected Location LocationOf(Object obj)
        {
            Location location;
            float horizontalDistance1 = Math.Abs(obj.X + obj.CollisionWidth - this.X);
            float horizontalDistance2 = Math.Abs(this.X + this.CollisionWidth - obj.X);
            float verticalDistance1 = Math.Abs(obj.Y + obj.CollisionHeight - this.Y);
            float verticalDistance2 = Math.Abs(this.Y + this.CollisionHeight - obj.Y);

            float horizontalDistance = Math.Min(horizontalDistance1, horizontalDistance2);
            float verticalDistance = Math.Min(verticalDistance1, verticalDistance2);

            if (verticalDistance < horizontalDistance)
            {
                float offsetTop = this.Y + this.CollisionHeight - obj.Y;
                float offsetBottom = offsetTop - obj.CollisionHeight;
                if (Math.Abs(offsetTop) < Math.Abs(offsetBottom))
                    location = Location.BELOW;
                else
                    location = Location.ABOVE;
            }
            else
            {
                float offsetLeft = this.X + this.CollisionWidth - obj.X;
                float offsetRight = offsetLeft - obj.CollisionWidth;
                if (Math.Abs(offsetLeft) < Math.Abs(offsetRight))
                    location = Location.RIGHT;
                else
                    location = Location.LEFT;
            }

            return location;
        }
    }

    public enum Location { NONE, ABOVE, BELOW, LEFT, RIGHT }
}
