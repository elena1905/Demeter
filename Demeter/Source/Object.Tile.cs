using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Demeter
{
    public abstract class Tile : Object
    {
        public BlockType type = BlockType.None;
        protected Animation movableBlock;

        private int width;
        protected int Width
        {
            get { return width; }
            set
            {
                width = value;
                tileFrame.X = (int)Math.Ceiling((double)width / texture.Width);
            }
        }

        private int height;
        protected int Height
        {
            get { return height; }
            set
            {
                height = value;
                tileFrame.Y = (int)Math.Ceiling((double)height / texture.Height);
            }
        }

        public override int CollisionWidth
        {
            get { return width; }
        }
        public override int CollisionHeight
        {
            get { return height; }
        }

        protected Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        Point tileFrame = new Point();

        public override Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)(position.X),
                    (int)(position.Y), CollisionWidth, CollisionHeight);
            }
        }

        public Tile(Game1 game)
            : base(game)
        {
        }

        public Tile(Game1 game, XmlTextReader reader)
            : base(game, reader)
        {
        }

        public Tile(Game1 game, Vector2 position, Point tileFrame)
            : base(game, position)
        {
            this.tileFrame = tileFrame;
        }

        public Tile(Game1 game, Vector2 position, int width)
            : base(game, position)
        {
            this.Width = width;
            this.Height = texture.Height;
        }

        public Tile(Game1 game, Vector2 position, int width, int height)
            : base(game, position)
        {
            this.Width = width;
            this.Height = height;
        }

        public override void Draw(GameTime gameTime)
        {
            if (type == BlockType.MovableBlock)
            {
                for (int i = 0; i < tileFrame.X; i++)
                {
                    for (int j = 0; j < tileFrame.Y; j++)
                    {
                        Vector2 pos = new Vector2(position.X + movableBlock.FrameSize.X * i, position.Y + movableBlock.FrameSize.Y * j);
                        Vector2 screenPos = Level.ScreenPosition(pos);
                        Game.SpriteBatch.Draw(movableBlock.Texture, screenPos,
                            movableBlock.CurrentSourceRectangle, Color.White,
                            0, movableBlock.Origin, 1.0f, SpriteEffects.None, 0.9f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < tileFrame.X; i++)
                {
                    for (int j = 0; j < tileFrame.Y; j++)
                    {
                        Vector2 pos = new Vector2(position.X + texture.Width * i, position.Y + texture.Height * j);
                        Vector2 screenPos = Level.ScreenPosition(pos);
                        if (type == BlockType.Ground)
                        {
                            Game.SpriteBatch.Draw(texture,
                                new Rectangle((int)screenPos.X, (int)screenPos.Y, texture.Width, (int)(Level.Height - position.Y)),
                                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.2f);
                        }
                        else
                        {
                            Game.SpriteBatch.Draw(texture,
                                new Rectangle((int)screenPos.X, (int)screenPos.Y, texture.Width, texture.Height),
                                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.2f);
                        }
                    }
                }
            }
        }
    }
    public enum BlockType { None, Ground, Wall, Floor, MovableBlock }
}
