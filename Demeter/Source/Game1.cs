using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

namespace Demeter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        private SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private Level level;
        public Level Level
        {
            get { return level; }
        }

        BindingPoint bindingPoint;
        public BindingPoint BindingPoint
        {
            get { return bindingPoint; }
            set { bindingPoint = value; }
        }

        int width = 1366;
        public int Width
        {
            get { return this.width; }
        }

        int height = 768;
        public int Height
        {
            get { return this.height; }
        }

        public int HalfWidth
        {
            get { return this.width / 2; }
        }

        public int HalfHeight
        {
            get { return this.height / 2; }
        }

        public SpriteFont font;

        private int dieTime;
        string current_levelFileName;
        public string Current_levelFileName
        {
            get { return current_levelFileName; }
        }
        Vector2 diePosition;
        public Vector2 DiePosition
        {
            get { return diePosition; }
        }

        string playerBornPointId;
        public string PlayerBornPointId
        {
            get { return playerBornPointId; }
        }

        #region menu relative
        bool gotoMenu = false;
        bool wasUpKeydown = false;
        bool wasDownKeydown = false;
        bool wasEscKeydown = false;

        int currentSelection = 1;
        const int maxItemsCount = 5;

        Vector2[] menuPos = new Vector2[maxItemsCount];
        #endregion

        #region music_relative

        Song soundEffect;
        bool musicOn;

        Texture2D menuBackground;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            if (!graphics.IsFullScreen)
                graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            for (int i = 0; i < maxItemsCount; i++)
            {
                menuPos[i] = new Vector2(550 , i*50 + 250);
            }
                // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            soundEffect = Content.Load<Song>(@"audio\backgroundMusic");

            // Start the soundtrack audio
            if (!musicOn)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(soundEffect);
                musicOn = true;
            }

            font = Content.Load<SpriteFont>("font/Hud");

            level = new Level(this);
            string startLevel = Xml.StartLevel;
            if (startLevel == null)
            {
                startLevel = "novice.xml";
            }
            level.Load(startLevel);

            menuBackground = Content.Load<Texture2D>("texture/Background.MenuBackground");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!wasEscKeydown)
                {
                    wasEscKeydown = true;
                    gotoMenu = !gotoMenu;
                }
            }
            else
            {
                wasEscKeydown = false;
            }

            if (!gotoMenu)
            {
                level.Update(gameTime);
                current_levelFileName = level.LevelFileName;

                if (level.Player.IsLeaving)
                {
                    ChangeLevel();

                        playerBornPointId = null;
                        playerBornPointId = Level.Player.BornPointId;

                }

                if (!level.Player.IsAlive)
                {
                    dieTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (dieTime > level.Player.DieTime)
                    {
                        diePosition = new Vector2(level.Player.Position.X,
                            level.Player.Position.Y);
                        level = new Level(this);
                        level.Load(current_levelFileName);
                        dieTime = 0;
                    }
                }
            }
            else
            {
                MenuUpdate();
            }

            base.Update(gameTime);
        }

        private void ChangeLevel()
        {
            string commingLevel = Level.Player.ComingLevel;

            if (commingLevel == "null")
            {
                return;
            }

            if (commingLevel == "TotalLevel.xml")
            {
                Xml.StartLevel = "TotalLevel.xml";
            }

            bindingPoint = null;

            level = new Level(this);
            level.Load(commingLevel);

            level.Player.IsLeaving = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            if (!gotoMenu)
            {
                currentSelection = 1;
                level.Draw(gameTime);
            }
            else
            {
                MenuDraw();
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void MenuUpdate()
        {
            // select options
            KeyboardState keystate = Keyboard.GetState();
            if (keystate.IsKeyDown(Keys.Down))
            {
                if (!wasDownKeydown)
                {
                    currentSelection++;
                    if (Level.LevelFileName.IndexOf("-") < 0 && currentSelection == 2)
                    {
                        currentSelection++;
                    }
                    if (currentSelection > maxItemsCount)
                    {
                        currentSelection = 1;
                    }
                }
                wasDownKeydown = true;
            }
            else
            {
                wasDownKeydown = false;
            }

            if (keystate.IsKeyDown(Keys.Up))
            {
                if (!wasUpKeydown)
                {
                    currentSelection--;
                    if (Level.LevelFileName.IndexOf("-") < 0 && currentSelection == 2)
                    {
                        currentSelection--;
                    }
                    if (currentSelection < 1)
                    {
                        currentSelection = maxItemsCount;
                    }
                }
                wasUpKeydown = true;
            }
            else
            {
                wasUpKeydown = false;
            }

            // execute the selectioon
            if (currentSelection == 1 && keystate.IsKeyDown(Keys.Enter))
            {
                gotoMenu = false;
            }
            else if (currentSelection == 2 && keystate.IsKeyDown(Keys.Enter))
            {
                if (Level.LevelFileName.IndexOf("-") > 0)
                {
                    bindingPoint = null;
                    gotoMenu = false;
                    String general_level = Level.LevelFileName.Substring(0, 6);
                    general_level += ".xml";
                    level = new Level(this);
                    level.Load(general_level);
                }
            }
            else if (currentSelection == 3 && keystate.IsKeyDown(Keys.Enter))
            {
                bindingPoint = null;
                Level.TreasureMgr.ResetLevel();
                gotoMenu = false;
                LoadContent();
            }
            else if (currentSelection == 4 && keystate.IsKeyDown(Keys.Enter))
            {
                if (musicOn)
                {
                    MediaPlayer.Pause();
                    musicOn = false;
                }
                else
                {
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(soundEffect);
                    musicOn = true;
                }
                gotoMenu = false;
            }
            else if (currentSelection == 5 && keystate.IsKeyDown(Keys.Enter))
            {
                this.Exit();
            }
        }

        public void MenuDraw()
        {
            spriteBatch.Draw(menuBackground, new Vector2(300, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            spriteBatch.DrawString(font, "DEMETER", new Vector2(550,100), Color.Black, 0, Vector2.Zero, 2, SpriteEffects.None, 0.9f);

            if (currentSelection != 1)
                spriteBatch.DrawString(font, "Resume Game", menuPos[0], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            if (currentSelection != 2)
            {
                if (Level.LevelFileName.IndexOf("-") < 0)
                {
                    spriteBatch.DrawString(font, "Leave Current Level", menuPos[1], Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                }
                else
                {
                    spriteBatch.DrawString(font, "Leave Current Level", menuPos[1], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                }
            }
            if (currentSelection != 3)
                spriteBatch.DrawString(font, "Restart Game", menuPos[2], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            if (currentSelection != 4)
            {
                if (musicOn)
                {
                    spriteBatch.DrawString(font, "Stop Background Music", menuPos[3], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                }
                else
                {
                    spriteBatch.DrawString(font, "Start Background Music", menuPos[3], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                }
            }
            if (currentSelection != 5)
                spriteBatch.DrawString(font, "Leave Demeter", menuPos[4], Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);

            switch (currentSelection)
            {
                case 1:
                    spriteBatch.DrawString(font, "Resume Game", menuPos[0], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case 2:
                    spriteBatch.DrawString(font, "Leave Current Level", menuPos[1], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case 3:
                    spriteBatch.DrawString(font, "Restart Game", menuPos[2], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
                case 4:
                    if (musicOn)
                    {
                        spriteBatch.DrawString(font, "Stop Background Music", menuPos[3], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Start Background Music", menuPos[3], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    }
                    break;
                case 5:
                    spriteBatch.DrawString(font, "Leave Demeter", menuPos[4], Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                    break;
            }
        }
    }
}
