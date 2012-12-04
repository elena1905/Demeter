using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace Demeter
{
    public class Level
    {
        #region fields

        string levelFileName;
        public string LevelFileName
        {
            get { return levelFileName; }
        }

        string topic;
        Vector2 topicPos;

        TreasureManager treasureMgr;
        public TreasureManager TreasureMgr
        {
            get { return this.treasureMgr; }
        }

        Game1 game;
        public Game1 Game
        {
            get { return this.game; }
        }

        Player player;
        public Player Player
        {
            get { return this.player; }
        }

        List<Object> objects = new List<Object>();
        public List<Object> Objects
        {
          get { return objects; }
          set { objects = value; }
        }

        List<Object> movableObjects = new List<Object>();
        public List<Object> MovableObjects
        {
            get { return movableObjects; }
            set { movableObjects = value; }
        }

        List<LightSource> lightSource = new List<LightSource>();

        string backgroundTextureAssetName;
        Texture2D backgroundTexture;
        string foregroundTextureAssetName;
        Texture2D foregroundTexture;
        Color? backgroundColor;

        int width;
        public int Width
        {
            get { return width; }
        }

        int height;
        public int Height
        {
            get { return height; }
        }

        int leftBound;
        int rightBound;
        int topBound;
        int bottomBound;

        Vector2 cameraOffset;
        public Vector2 CameraOffset
        {
            get { return this.cameraOffset; }
        }

        public Vector2 ScreenPosition(Vector2 pos)
        {
            return new Vector2(pos.X - CameraOffset.X,
               pos.Y - CameraOffset.Y);
        }

        GridManager gridManager;

        bool leavingWithoutTurningOff = false;
        Vector2 hintWordsOffset = new Vector2(40, 35);
        Texture2D hint;
        Texture2D hintWords;
        int timeEmerging;
        const int intervalTime = 5000;

        bool isTotalLevel = false;
        Texture2D plant1;
        int gottenTreasure;
        int totalTreasure;
        public int GottenTreasure
        {
            get { return gottenTreasure; }
            set {
                gottenTreasure = value;
                SelectPlantTexture();
            }
        }        

        bool showHint = false;
        Animation leftHint;
        Animation rightHint;
        int timeEmerging2;
        const int intervalTime2 = 5000;

        #endregion

        public Level(Game1 game)
        {
            this.game = game;
        }

        #region xml_Load
        public void Load(string levelFileName)
        {
            this.levelFileName = levelFileName;

            List<string> controllerId = new List<string>();
            List<string> controlledId = new List<string>();

            XmlTextReader reader = new XmlTextReader("Content/level/" + levelFileName);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "size")
                    {
                        string width = reader.GetAttribute("width");
                        string height = reader.GetAttribute("height");
                        string treasureCountStr = reader.GetAttribute("treasureCount");

                        int treasureCount = int.Parse(treasureCountStr);
                        this.width = int.Parse(width);
                        this.height = int.Parse(height);

                        this.treasureMgr = new TreasureManager(this.levelFileName, treasureCount);
                    }
                    else if (reader.Name == "background")
                    {
                        backgroundTextureAssetName = reader.GetAttribute("texture");
                        string backgroundColorStr = reader.GetAttribute("color");
                        if (backgroundColorStr == "black")
                            backgroundColor = Color.Black;
                    }
                    else if (reader.Name == "foreground")
                    {
                        foregroundTextureAssetName = reader.GetAttribute("texture");
                        string bottom = reader.GetAttribute("bottom");
                    }
                    else if (reader.Name == "bound")
                    {
                        string leftStr = reader.GetAttribute("left");
                        string rightStr = reader.GetAttribute("right");
                        string topStr = reader.GetAttribute("top");
                        string bottomStr = reader.GetAttribute("bottom");
                        leftBound = int.Parse(leftStr);
                        rightBound = int.Parse(rightStr);
                        topBound = int.Parse(topStr);
                        bottomBound = int.Parse(bottomStr);
                        Block leftBoundBlock = new Block(game, new Vector2(leftBound, 0), 0, height);
                        Block rightBoundBlock = new Block(game, new Vector2(rightBound, 0), 0, height);
                        Block topBoundBlock = new Block(game, new Vector2(0, topBound), width, 0);
                        Block bottomBoundBlock = new Block(game, new Vector2(0, bottomBound), width, 0);
                        objects.Add(leftBoundBlock);
                        objects.Add(rightBoundBlock);
                        objects.Add(topBoundBlock);
                        objects.Add(bottomBoundBlock);
                    }
                    else if (reader.Name == "player")
                    {
                        player = new Player(game, reader);
                    }
                    else if (reader.Name == "enemy")
                    {
                        Enemy enemy = new Enemy(game, reader);
                    }
                    else if (reader.Name == "shiftStick")
                    {
                        ShiftStick stick = new ShiftStick(game, reader);

                        XmlReader subtree = reader.ReadSubtree();
                        while (subtree.Read())
                        {
                            if (subtree.NodeType == XmlNodeType.Element &&
                                subtree.Name == "controlled")
                            {
                                controllerId.Add(stick.Id);
                                controlledId.Add(subtree.GetAttribute("objId"));
                            }
                        }
                    }
                    else if (reader.Name == "lightsource")
                    {
                        LightSource light1 = new LightSource(game, reader);
                        lightSource.Add(light1);
                    }
                    else if (reader.Name == "mirror")
                    {
                        Mirror mirror1 = new Mirror(game, reader);
                    }
                    else if (reader.Name == "switch")
                    {
                        Switch switch1 = new Switch(game, reader);

                        XmlReader subtree = reader.ReadSubtree();
                        while (subtree.Read())
                        {
                            if (subtree.NodeType == XmlNodeType.Element &&
                                subtree.Name == "controlled")
                            {
                                controllerId.Add(switch1.Id);
                                controlledId.Add(subtree.GetAttribute("objId"));
                            }
                        }
                    }
                    else if (reader.Name == "ladder")
                    {
                        Ladder ladder = new Ladder(game, reader);
                    }
                    else if (reader.Name == "door")
                    {
                        Door door = new Door(game, reader);
                    }
                    else if (reader.Name == "platform")
                    {
                        Platform platform = new Platform(game, reader);
                    }
                    else if (reader.Name == "block")
                    {
                        Block block = new Block(game, reader);
                    }
                    else if (reader.Name == "launcher")
                    {
                        Launcher launcher = new Launcher(game, reader);
                    }
                    else if (reader.Name == "treasure")
                    {
                        Treasure treasure;
                        bool flag = false;
                        List<String> ids = this.treasureMgr.AreGotten();
                        foreach (String id in ids)
                        {
                            if (reader.GetAttribute("id") == id)
                            {
                                treasure = new Treasure(game, reader, true);
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            treasure = new Treasure(game, reader, false);
                        }
                    }
                    else if (reader.Name == "deadlyobj")
                    {
                        DeadlyObject deadlyobj = new DeadlyObject(game, reader);
                    }
                    else if (reader.Name == "ogre")
                    {
                        Ogre ogre = new Ogre(game, reader);
                    }
                    else if (reader.Name == "gate")
                    {
                        Gate gate = new Gate(game, reader);
                    }
                    else if (reader.Name == "topic")
                    {
                        string pxStr = reader.GetAttribute("px");
                        string pyStr = reader.GetAttribute("py");

                        topic = reader.ReadElementString();

                        float px = float.Parse(pxStr);
                        float py = float.Parse(pyStr);
                        topicPos = new Vector2(px, py);
                    }
                    else if (reader.Name == "bindingPoint")
                    {
                        if (Game.BindingPoint == null)
                        {
                            Game.BindingPoint = new BindingPoint();
                            XmlReader subtree = reader.ReadSubtree();
                            while (subtree.Read())
                            {
                                if (subtree.NodeType == XmlNodeType.Element &&
                                subtree.Name == "p")
                                {
                                    string pxStr = subtree.GetAttribute("px");
                                    string pyStr = subtree.GetAttribute("py");
                                    string id = subtree.GetAttribute("id");

                                    Vector2 pos = new Vector2(int.Parse(pxStr), int.Parse(pyStr));
                                    Game.BindingPoint.Add(pos, levelFileName, id);
                                }
                            }
                        }
                    }
                }
            }

            reader.Close();

            List<string>.Enumerator controllerEnum = controllerId.GetEnumerator();
            List<string>.Enumerator controlledEnum = controlledId.GetEnumerator();
            while (controllerEnum.MoveNext() && controlledEnum.MoveNext())
            {
                string controllerStr = controllerEnum.Current;
                string controlledStr = controlledEnum.Current;
                IController controller = (IController)GetObjectById(controllerStr);
                IControlledObject controlled = (IControlledObject)GetObjectById(controlledStr);
                controller.Add(controlled);
            }

            Initialize();
            LoadContent();
        }
        #endregion

        public void Initialize()
        {
            InitializeGridManager();

            if (levelFileName == "TotalLevel.xml")
            {
                isTotalLevel = true;
            }
            treasureMgr.GetTreasureNum("level1.xml", out gottenTreasure, out totalTreasure);
            SelectPlantTexture();

            if (levelFileName == "novice.xml")
            {
                showHint = true;
                timeEmerging2 = intervalTime2;
                leftHint = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Hint.Left"), new Point(35, 32), 100, true);
                rightHint = new Animation(Game.Content.Load<Texture2D>("texture/Object.Sprite.Hint.Right"), new Point(35, 32), 100, true);
            }
        }

        private void SelectPlantTexture()
        {
            switch (gottenTreasure)
            {
                case 0:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-0");
                    break;
                case 1:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-1");
                    break;
                case 2:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-2");
                    break;
                case 3:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-3");
                    break;
                case 4:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-4");
                    break;
                case 5:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-5");
                    break;
                case 6:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-6");
                    break;
                case 7:
                    plant1 = Game.Content.Load<Texture2D>("texture/Plant1-7");
                    break;
            }
        }

        public void LoadContent()
        {
            if (backgroundTextureAssetName != null && backgroundTextureAssetName != "null")
            {
                backgroundTexture = Game.Content.Load<Texture2D>(backgroundTextureAssetName);
            }
            if (foregroundTextureAssetName != null && foregroundTextureAssetName != "null")
            {
                foregroundTexture = Game.Content.Load<Texture2D>(foregroundTextureAssetName);
            }
            hint = Game.Content.Load<Texture2D>("texture/Hint.TurnOff");
            hintWords = Game.Content.Load<Texture2D>("texture/Hint.TurnOffWords");
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < movableObjects.Count; i++)
            {
                movableObjects[i].Update(gameTime);
            }

            foreach (Object obj in objects)
            {
                obj.Update(gameTime);
            }

            SetCamera();

            if (timeEmerging > 0)
            {
                timeEmerging -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeEmerging <= 0)
                    leavingWithoutTurningOff = false;
            }

            if (player.IsLeaving)
            {
                foreach (LightSource light in lightSource)
                {
                    if (light.SwitchOn)
                    {
                        leavingWithoutTurningOff = true;
                        timeEmerging = intervalTime;
                        player.IsLeaving = false;
                    }
                }
            }

            if (showHint)
            {
                leftHint.Update(gameTime);
                rightHint.Update(gameTime);
            }
        }

        private void SetCamera()
        {
            if (player.Position.X < Game.HalfWidth)
            {
                cameraOffset.X = 0;
            }
            else if (player.Position.X > width - Game.HalfWidth)
            {
                cameraOffset.X = width - game.Width;
            }
            else
            {
                cameraOffset.X = player.Position.X - Game.HalfWidth;
            }

            if (player.Position.Y < Game.HalfHeight)
            {
                cameraOffset.Y = 0;
            }
            else if (player.Position.Y > height - Game.HalfHeight)
            {
                cameraOffset.Y = height - game.Height;
            }
            else
            {
                cameraOffset.Y = player.Position.Y - Game.HalfHeight;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (backgroundColor != null)
            {
                Game.GraphicsDevice.Clear(backgroundColor.Value);
            }

            Vector2 p = ScreenPosition(Vector2.Zero);

            if (backgroundTexture != null)
            {
                for (int i = 0; i < (int)Math.Ceiling((float)width / backgroundTexture.Width); i++)
                {
                    Game.SpriteBatch.Draw(backgroundTexture,
                        new Rectangle((int)(p.X * 0.5 + backgroundTexture.Width * i), (int)(p.Y), backgroundTexture.Width, height),
                        null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                }
            }

            if (foregroundTexture != null)
            {
                for (int i = 0; i < (int)Math.Ceiling((float)width / foregroundTexture.Width); i++)
                {
                    Game.SpriteBatch.Draw(foregroundTexture,
                        new Rectangle((int)(p.X + foregroundTexture.Width * i), height - foregroundTexture.Height, foregroundTexture.Width, foregroundTexture.Height), null,
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.11f);
                }
            }

            if (topic != null)
            {
                Game.SpriteBatch.DrawString(Game.font, topic, topicPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            }

            foreach (Object obj in movableObjects)
            {
                obj.Draw(gameTime);
            }

            foreach (Object obj in objects)
            {
                obj.Draw(gameTime);
            }

            if (leavingWithoutTurningOff)
            {
                Vector2 screenPos = player.ScreenPosition;
                Vector2 pos;
                SpriteEffects effect;
                if (player.X + 600 > width)
                {
                    pos = new Vector2(screenPos.X - 190, screenPos.Y - 170);
                    effect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    pos = new Vector2(screenPos.X + 10, screenPos.Y - 170);
                    effect = SpriteEffects.None;
                }

                Game.SpriteBatch.Draw(hint, pos,
                    null, Color.White, 0, Vector2.Zero, 1, effect, 0.8f);
                Game.SpriteBatch.Draw(hintWords, pos + hintWordsOffset,
                    null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.81f);
            }

            if (showHint && timeEmerging2 > 0)
            {
                timeEmerging2 -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeEmerging2 <= 0)
                {
                    showHint = false;
                }
                Vector2 screenPos = player.ScreenPosition;
                Vector2 pos = new Vector2(screenPos.X-10,screenPos.Y-40);
                Vector2 pos2 = new Vector2(screenPos.X+30,screenPos.Y-40);
                game.SpriteBatch.Draw(leftHint.Texture, pos, leftHint.CurrentSourceRectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.8f);
                game.SpriteBatch.Draw(rightHint.Texture, pos2, leftHint.CurrentSourceRectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.8f);
            }

            if (isTotalLevel)
            {
                Game.SpriteBatch.Draw(plant1, new Vector2(600, 240), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                Game.SpriteBatch.Draw(plant1, new Vector2(1200, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            }
        }

        #region collision detection

        public void InitializeGridManager()
        {
            gridManager = new GridManager(width / GridManager.GridSize + 1, height / GridManager.GridSize + 1);
            foreach (Object obj in objects)
            {
                Rectangle rect = obj.CollisionRect;
                int leftIndex = rect.Left / GridManager.GridSize;
                int rightIndex = rect.Right / GridManager.GridSize;
                int topIndex = rect.Top / GridManager.GridSize;
                int bottomIndex = rect.Bottom / GridManager.GridSize;

                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    for (int j = topIndex; j <= bottomIndex; j++)
                    {
                        try
                        {
                            gridManager.Add(obj, i, j);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public List<Object> CollidedWith(Object obj)
        {
            List<Object> collided = new List<Object>();

            Rectangle rect = obj.CollisionRect;
            int leftIndex = rect.Left / GridManager.GridSize;
            int rightIndex = rect.Right / GridManager.GridSize;
            int topIndex = rect.Top / GridManager.GridSize;
            int bottomIndex = rect.Bottom / GridManager.GridSize;

            for (int i = leftIndex; i <= rightIndex; i++)
            {
                for (int j = topIndex; j <= bottomIndex; j++)
                {
                    try
                    {
                        List<Object> objectsInside = gridManager[i, j];
                        foreach (Object objectInside in objectsInside)
                        {
                            if (obj != objectInside && rect.Intersects(objectInside.CollisionRect))
                            {
                                collided.Add(objectInside);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new NotSupportedException("Player is out of bound");//todo
                    }
                }
            }

            foreach (Object movableObject in movableObjects)
            {
                if(obj!=movableObject && rect.Intersects(movableObject.CollisionRect))
                {
                    collided.Add(movableObject);
                }
            }

            return collided;
        }

        public Object FindObject(Vector2 pos, float angle, out Vector2? collidingPosition, Object exclusion)
        {
            Ray ray = new Ray(pos, angle);

            Object nearestMovableObj = null;
            Vector2? nearestMovablePoint = null;
            Object nearestObj = null;
            Vector2? nearestPoint = null;

            foreach (Object obj in movableObjects)
            {
                if (obj != exclusion)
                {
                    List<Vector2> intersection = ray.Intersects(obj.CollisionRect, true);
                    if (intersection != null && intersection.Count > 0)
                    {
                        Vector2 point = intersection.First();
                        if (nearestMovableObj == null || (pos - point).Length() < (pos - nearestMovablePoint.Value).Length())
                        {
                            nearestMovableObj = obj;
                            nearestMovablePoint = point;
                        }
                    }
                }
            }

            int x = (int)(pos.X / GridManager.GridSize);
            int y = (int)(pos.Y / GridManager.GridSize);

            List<Object> objs;

            while ((objs = gridManager[x, y]) != null)
            {
                Rectangle currentGrid = gridManager.GetRect(x, y);

                foreach (Object obj in objs)
                {
                    if (obj != exclusion)
                    {
                        List<Vector2> intersection = ray.Intersects(obj.CollisionRect, true);

                        if (intersection == null || intersection.Count == 0)
                        {
                            continue;
                        }

                        Vector2 point = intersection.First();

                        if (Helper.Contains(currentGrid, point) &&
                            (nearestObj == null || (pos - point).Length() < (pos - nearestPoint.Value).Length()))
                        {
                            nearestObj = obj;
                            nearestPoint = point;
                        }
                    }
                }

                if (nearestObj != null)
                {
                    break;
                }

                Point next = NextGrid(ray, currentGrid);
                x += next.X;
                y += next.Y;
            }

            if (nearestMovableObj != null &&
                (pos - nearestMovablePoint.Value).Length() < (pos - nearestPoint.Value).Length())
            {
                collidingPosition = nearestMovablePoint;
                return nearestMovableObj;
            }
            else
            {
                collidingPosition = nearestPoint;
                return nearestObj;
            }
        }

        private Point NextGrid(Ray ray, Rectangle currentGrid)
        {
            Point next = new Point();

            List<Vector2> intersections = ray.Intersects(currentGrid, true);
            Vector2 intersection = (intersections.Count == 1)
                ? intersections.First() : intersections[1];

            if (intersection.X == currentGrid.Left)
                next.X = -1;
            else if (intersection.X == currentGrid.Right)
                next.X = 1;
            if (intersection.Y == currentGrid.Top)
                next.Y = -1;
            else if (intersection.Y == currentGrid.Bottom)
                next.Y = 1;

            return next;
        }

        #endregion

        public Object GetObjectById(string id)
        {
            foreach (Object obj in objects)
            {
                if (obj.Id == id)
                    return obj;
            }
            foreach (Object obj in movableObjects)
            {
                if (obj.Id == id)
                    return obj;
            }
            return null;
        }
    }

    public class Grid
    {
        private List<Object> objectInside;

        public List<Object> ObjectInside
        {
            get { return objectInside; }
        }

        public Grid()
        {
            objectInside = new List<Object>();
        }

        public void Add(Object obj)
        {
            objectInside.Add(obj);
        }
    }

    public class GridManager
    {
        public static readonly int GridSize = 50;

        private Grid[,] manager;
        private int width;
        private int height;

        public GridManager(int width, int height)
        {
            this.width = width;
            this.height = height;

            manager = new Grid[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    manager[i, j] = new Grid();
                }
            }
        }

        public List<Object> this[int x, int y]
        {
            get
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                    return manager[x, y].ObjectInside;
                else
                    return null;
            }
        }

        public Rectangle GetRect(int x, int y)
        {
            return new Rectangle(x * GridSize, y * GridSize, GridSize, GridSize);
        }

        public void Add(Object obj, int x, int y)
        {
            manager[x, y].Add(obj);
        }
    }
}
