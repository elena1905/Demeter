using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public class Gate : Tile, IControlledObject
    {
        protected BlockInfo blockInfo = null;
        public BlockInfo BlockInfo
        {
            get { return blockInfo; }
            set { blockInfo = value; }
        }

        public Gate(Game1 game, Vector2 position, Point tileFrame)
            : base(game, position, tileFrame)
        {
        }

        public Gate(Game1 game, Vector2 position, int width)
            : base(game, position, width)
        {
        }

        public Gate(Game1 game, Vector2 position, int width, int height)
            : base(game, position, width, height)
        {
        }

        public Gate(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string widthStr = reader.GetAttribute("width");
            string heightStr = reader.GetAttribute("height");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            int width = int.Parse(widthStr);
            int height = int.Parse(heightStr);

            this.position = new Vector2(px, py);
            this.Width = width;
            this.Height = height;

            XmlReader subtree = reader.ReadSubtree();
            while (subtree.Read())
            {
                if (subtree.NodeType == XmlNodeType.Element)
                {
                    if (subtree.Name == "bound")
                    {
                        string leftStr = reader.GetAttribute("left");
                        string rightStr = reader.GetAttribute("right");
                        string topStr = reader.GetAttribute("top");
                        string bottomStr = reader.GetAttribute("bottom");
                        float left = float.Parse(leftStr);
                        float right = float.Parse(rightStr);
                        float top = float.Parse(topStr);
                        float bottom = float.Parse(bottomStr);
                        this.blockInfo = new BlockInfo(left, right, top, bottom, true);
                    }
                }
            }
        }

        public override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.Wall");
        }

        public override void Update(GameTime gameTime)
        {
            if (blockInfo != null && blockInfo.Moving == true)
            {
                List<Object> collided = Level.CollidedWith(this);
                List<Object> above = new List<Object>();
                foreach (Object obj in collided)
                {
                    Location location = LocationOf(obj);
                    if (location == Location.ABOVE)
                    {
                        above.Add(obj);
                    }
                }

                if (blockInfo.Positive)
                {
                    position += blockInfo.Speed;
                    foreach (Object obj in above)
                    {
                        obj.Position += blockInfo.Speed;
                    }
                }
                else
                {
                    position -= blockInfo.Speed;
                    foreach (Object obj in above)
                    {
                        obj.Position -= blockInfo.Speed;
                    }
                }

                // out of bound
                if (position.X < blockInfo.LeftBound ||
                    position.X > blockInfo.RightBound ||
                    position.Y < blockInfo.TopBound ||
                    position.Y > blockInfo.BottomBound)
                {
                    blockInfo.Moving = false;
                }

                CollisionDetection();
            }
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

        #region IControlledObject Members

        bool haveControlled = false;
        void IControlledObject.Control(IController controller)
        {
            if (!haveControlled && blockInfo != null)
            {
                blockInfo.Positive = !blockInfo.Positive;
                blockInfo.Moving = true;
                haveControlled = true;
            }
        }

        #endregion
    }

    public class BlockInfo
    {
        const float MaxSpeed = 1.0f;

        readonly Vector2 speed;
        public Vector2 Speed
        {
            get { return speed; }
        }

        bool positive = false;
        public bool Positive
        {
            get { return positive; }
            set { positive = value; }
        }

        bool moving = false;
        public bool Moving
        {
            get { return moving; }
            set { moving = value; }
        }

        float leftBound;
        public float LeftBound
        {
            get { return leftBound; }
            set { leftBound = value; }
        }

        float rightBound;
        public float RightBound
        {
            get { return rightBound; }
            set { rightBound = value; }
        }

        float topBound;
        public float TopBound
        {
            get { return topBound; }
            set { topBound = value; }
        }

        float bottomBound;
        public float BottomBound
        {
            get { return bottomBound; }
            set { bottomBound = value; }
        }

        public BlockInfo(float leftBound, float rightBound, float topBound, float bottomBound, bool positive)
        {
            this.leftBound = leftBound;
            this.rightBound = rightBound;
            this.topBound = topBound;
            this.bottomBound = bottomBound;
            this.positive = positive;

            speed = new Vector2();
            float offsetX = Math.Abs(leftBound - rightBound);
            float offsetY = Math.Abs(topBound - bottomBound);
            if (offsetX > offsetY)
            {
                speed.X = MaxSpeed;
                speed.Y = MaxSpeed / offsetX * offsetY;
            }
            else
            {
                speed.Y = MaxSpeed;
                speed.X = MaxSpeed / offsetY * offsetX;
            }
        }
    }
}
