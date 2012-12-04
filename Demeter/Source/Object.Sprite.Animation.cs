using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demeter
{
    public class Animation
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        Texture2D texture;
        public Texture2D Texture
        {
            get { return this.texture; }
        }

        /// <summary>
        /// Gets the size of each frame in the animation.
        /// </summary>
        protected Point frameSize;
        public Point FrameSize
        {
            get { return this.frameSize; }
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        protected Point frameCount;
        public Point FrameCount
        {
            get { return this.frameCount; }
        }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        protected Point currentFrame;
        public Point CurrentFrame
        {
            get { return this.currentFrame; }
        }
        protected static readonly Point DEFAULT_FRAME = Point.Zero;

        float time = 0;

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            get { return texture.Width; }
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return texture.Height; }
        }

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return Vector2.Zero; }
        }

        /// <summary>
        /// Gets the source rectangle of the current frame.
        /// </summary>
        public Rectangle CurrentSourceRectangle
        {
            get
            {
                return new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X,
                    frameSize.Y);
            }
        }

        /// <summary>
        /// Duration of time to show each frame using milliseconds.
        /// </summary>
        int frameTime;
        public int FrameTime
        {
            get { return this.frameTime; }
        }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        bool isLooping;
        public bool IsLooping
        {
            get { return this.isLooping; }
        }

        bool loopFlag;

        /// <summary>
        /// Constructors a new animation.
        /// </summary>        
        public Animation(Texture2D texture, Point frameSize, int frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameSize = frameSize;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.frameCount = new Point(FrameWidth / frameSize.X, FrameHeight / frameSize.Y);
            loopFlag = true;
        }

        public void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.Milliseconds;
            while (time > FrameTime)
            {
                time -= FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (isLooping || loopFlag)
                {
                    ++currentFrame.X;
                    if (currentFrame.X >= frameCount.X)
                    {
                        if (currentFrame.X == frameCount.X && currentFrame.Y == frameCount.Y)
                        {
                            loopFlag = false;
                        }
                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= frameCount.Y)
                        {
                            currentFrame.Y = 0;
                        }
                    }
                }
            }
        }
    }
}
