using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public class Block : Tile, IControlledObject
    {
        protected BlockInfo blockInfo = null;
        public BlockInfo BlockInfo
        {
            get { return blockInfo; }
            set { blockInfo = value; }
        }

        public Block(Game1 game, Vector2 position, Point tileFrame)
            : base(game, position, tileFrame)
        {
        }

        public Block(Game1 game, Vector2 position, int width)
            : base(game, position, width)
        {
        }

        public Block(Game1 game, Vector2 position, int width, int height)
            : base(game, position, width, height)
        {
        }

        public Block(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string widthStr = reader.GetAttribute("width");
            string heightStr = reader.GetAttribute("height");
            string typeStr = reader.GetAttribute("type");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            int width = int.Parse(widthStr);
            int height = int.Parse(heightStr);

            this.position = new Vector2(px, py);
            this.Width = width;
            this.Height = height;

            if (typeStr == "ground")
            {
                type = BlockType.Ground;
            }
            else if (typeStr == "floor")
            {
                type = BlockType.Floor;
            }
            else if (typeStr == "wall")
            {
                type = BlockType.Wall;
            }
            else if (typeStr == "movableBlock")
            {
                type = BlockType.MovableBlock;
            }


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
                        string positiveStr = reader.GetAttribute("positive");
                        
                        float left = float.Parse(leftStr);
                        float right = float.Parse(rightStr);
                        float top = float.Parse(topStr);
                        float bottom = float.Parse(bottomStr);                        

                        bool positive = (positiveStr == "true") ? true : false;
                        this.blockInfo = new BlockInfo(left, right, top, bottom, positive);
                    }
                }
            }

            LoadContent();
        }

        public override void LoadContent()
        {
            switch (type)
            {
                case BlockType.Floor:
                    texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.Wall");
                    break;
                case BlockType.Ground:
                    texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.Ground");
                    break;
                case BlockType.Wall:
                    texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.Wall");
                    break;
                case BlockType.MovableBlock:
                    texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.MovableBlock2");
                    movableBlock = new Animation(texture, new Point(64, 48), 100, true);
                    break;
                default:
                    texture = Game.Content.Load<Texture2D>("texture/Object.Tile.Block.Wall");
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (type == BlockType.MovableBlock)
            {
                movableBlock.Update(gameTime);
            }
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

        void IControlledObject.Control(IController controller)
        {
            blockInfo.Positive = !blockInfo.Positive;
            blockInfo.Moving = true;
        }

        #endregion
    }
}
