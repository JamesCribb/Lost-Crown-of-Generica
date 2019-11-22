using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroWalk
{
    public static class RNG
    {
        public static Random rand;
        public static void Init()
        {
            rand = new Random();
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // If true, player starts outside the house, with the sword
        private bool debugStart = false;

        // used to highlight bounding boxes, for debugging
        private Texture2D whiteTexture;
        // currently used in the HUD, probably also used for debugging
        private SpriteFont arial;

        private Player hero;

        public Level[][] levelMap;
        public Dictionary<string, List<Level>> zones;

        public Dictionary<string, Texture2D> tiles;
        //public List<string> tileFileNames;
        private Dictionary<string, List<Rectangle>> tileBoundingBoxes =
            new Dictionary<string, List<Rectangle>>
            {
                { "CliffFaceCenter1", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceCenter2", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceCenter3", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceLeft1", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceLeft2", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceLeft3", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceRight1", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceRight2", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },
                { "CliffFaceRight3", new List<Rectangle>{new Rectangle(0, 0, 16, 16) } },

                { "CliffCorner1", new List<Rectangle>{new Rectangle(13, 13, 3, 3) } },
                { "CliffCorner2", new List<Rectangle>{new Rectangle(0, 13, 3, 3) } },
                { "CliffCorner3", new List<Rectangle>{new Rectangle(13, 0, 3, 3) } },
                { "CliffCorner4", new List<Rectangle>{new Rectangle(0, 0, 3, 3) } },

                { "CliffEdge1", new List<Rectangle>{new Rectangle(0, 5, 3, 11) } },
                { "CliffEdge2", new List<Rectangle>{new Rectangle(0, 0, 3, 16) } },
                { "CliffEdge3", new List<Rectangle>{new Rectangle(0, 0, 3, 16),
                    new Rectangle(0, 13, 16, 3) } },
                { "CliffEdge4", new List<Rectangle>{new Rectangle(0, 13, 16, 3) } },
                { "CliffEdge5", new List<Rectangle>{new Rectangle(13, 0, 3, 16),
                    new Rectangle(0, 13, 16, 3) } },
                { "CliffEdge6", new List<Rectangle>{new Rectangle(13, 0, 3, 16) } },
                { "CliffEdge7", new List<Rectangle>{new Rectangle(13, 5, 3, 11) } },
                { "CliffEdge8", new List<Rectangle>{new Rectangle(0, 0, 3, 16),
                    new Rectangle(0, 0, 16, 3) } },
                { "CliffEdge9", new List<Rectangle>{new Rectangle(0, 0, 16, 3) } },
                { "CliffEdge10", new List<Rectangle>{new Rectangle(0, 0, 16, 3),
                    new Rectangle(13, 0, 3, 6) } },

                { "CliffFaceShadow1", new List<Rectangle>{new Rectangle(13, 0, 3, 16) } },

                { "Shoreline1", new List<Rectangle>{ new Rectangle(0, 0, 3, 16),
                    new Rectangle(0, 0, 16, 3) } },
                { "Shoreline2", new List<Rectangle>{ new Rectangle(0, 0, 16, 3),
                    new Rectangle(13, 0, 3, 6) } },
                { "Shoreline3", new List<Rectangle>{ new Rectangle(13, 0, 3, 16),
                    new Rectangle(0, 13, 16, 3) } },
                { "Shoreline4", new List<Rectangle>{ new Rectangle(0, 0, 3, 16),
                    new Rectangle(0, 13, 16, 3) } },
                { "Shoreline5", new List<Rectangle>{ new Rectangle(13, 13, 3, 3) } },
                { "Shoreline6", new List<Rectangle>{ new Rectangle(0, 13, 16, 3) } },
                { "Shoreline7", new List<Rectangle>{ new Rectangle(0, 13, 3, 3) } },
                { "Shoreline8", new List<Rectangle>{ new Rectangle(13, 0, 3, 16) } },
                { "Shoreline9", new List<Rectangle>{ new Rectangle(0, 0, 3, 16) } },
                { "Shoreline10", new List<Rectangle>{ new Rectangle(0, 0, 3, 3) } },
                { "Shoreline11", new List<Rectangle>{ new Rectangle(0, 0, 16, 3) } },
                { "Shoreline12", new List<Rectangle>{ new Rectangle(13, 0, 3, 3) } },

                { "HouseWallBlack1", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack2", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack3", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack4", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack5", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack6", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack7", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlack8", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },

                { "HouseWallBlackCorner1", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlackCorner2", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlackCorner3", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallBlackCorner4", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },

                { "HouseWallWood1", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallWood2", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "HouseWallWood3", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                
                { "CaveWall1", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall2", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall3", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall4", new List<Rectangle>{ new Rectangle(0, 6, 16, 10) } },
                { "CaveWall5", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall6", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall7", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },
                { "CaveWall8", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } },

                { "CaveCorner1", new List<Rectangle>{ new Rectangle(2, 0, 14, 13) } },
                { "CaveCorner2", new List<Rectangle>{ new Rectangle(3, 6, 13, 10) } },
                { "CaveCorner3", new List<Rectangle>{ new Rectangle(0, 0, 14, 13) } },
                { "CaveCorner4", new List<Rectangle>{ new Rectangle(0, 6, 13, 10) } },

                { "Water1", new List<Rectangle>{ new Rectangle(0, 0, 16, 16) } }
        };

        public Dictionary<string, Prop> propTemplates;
        public Dictionary<string, Mobile> mobileTemplates;
        public Dictionary<string, CollectibleProp> collectibleTemplates;
        public Dictionary<string, InteractibleProp> interactibleTemplates;

        public KeyboardState prevKS;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // TEST: Set the game to run at 20fps
            // This is a good way of making sure that things aren't bound to the framerate
            // ...but it's very likely that they are
            //TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 50);

            // Set the screen size
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 672;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LineBatch.init(GraphicsDevice);
            RNG.Init();

            whiteTexture = Content.Load<Texture2D>("testing");
            arial = Content.Load<SpriteFont>("Arial");

            LoadHero();

            // Create the list of tiles
            tiles = new Dictionary<string, Texture2D>();
            foreach (var file in Directory.EnumerateFiles(@"Content\Tiles"))
            {
                string fileName = file.Split('\\')[2].Split('.')[0];
                tiles.Add(fileName, Content.Load<Texture2D>($@"Tiles\{fileName}"));
            }

            // Create the dictionary of worldObject templates
            LoadPropTemplates();

            // Create the dictionary of Mobile templates
            LoadMobileTemplates();

            // Create the dictionary of Collectible templates
            LoadCollectibleTemplates();

            // Create the dictionary of Interactible templates
            LoadInteractibleTemplates();

            // Read the levels in from file
            zones = new Dictionary<string, List<Level>>();
            foreach (var folder in Directory.EnumerateDirectories(@"TestLevels"))
            {
                string folderName = folder.Split('\\')[1];
                string zone = folderName.Split('-')[0];
                string x = folderName.Split('-')[1];
                string y = folderName.Split('-')[2];

                PopulateLevel(folderName, zone, int.Parse(x), int.Parse(y));
            }

            // Initialise the HUD
            var hud = new HeadUpDisplay(Vector2.Zero, 
                Content.Load<Texture2D>("hudBackground"),
                new Rectangle(0, 0, 640, 32),
                Content.Load<Texture2D>(@"HUD\heart100"),
                Content.Load<Texture2D>(@"HUD\heart75"),
                Content.Load<Texture2D>(@"HUD\heart50"),
                Content.Load<Texture2D>(@"HUD\heart25"),
                Content.Load<Texture2D>(@"HUD\heart0"),
                Content.Load<Texture2D>(@"HUD\rupee"),
                Content.Load<SpriteFont>("Consolas"), hero);

            if (debugStart)
            {
                // Start outside house
                LevelManager.Init(zones, "Overworld", new Vector2(1, 0), hud, collectibleTemplates,
                    new Vector2(320, 320));
                // Start with sword
                LevelManager.CurrentLevel.Hero.HasSword = true;
            }
            else
            {
                // Start inside house
                LevelManager.Init(zones, "Home", new Vector2(0, 0), hud, collectibleTemplates,
                    new Vector2(152, 164));
            }

            //// DEBUG:
            //// Start outside final cave
            //LevelManager.Init(zones, "Overworld", new Vector2(2, 2), hud, collectibleTemplates,
            //    new Vector2(320, 320));

            //// DEBUG: Start in the crown room
            //LevelManager.Init(zones, "CrownCave", new Vector2(0, 0), hud, collectibleTemplates,
            //    new Vector2(320, 320));
            

            // Initialise the zone-song dictionary
            Song dungeonSong = Content.Load<Song>(@"Music\Dungeon");
            var songs = new Dictionary<string, Song>
            {
                { "Title", Content.Load<Song>(@"Music\Title") },
                { "Victory", Content.Load<Song>(@"Music\PeacefulVictory") },
                { "Defeat", Content.Load<Song>(@"Music\Defeat") },
                { "Overworld", Content.Load<Song>(@"Music\Overworld") },
                { "SwordCave", dungeonSong },
                { "CrownCave", dungeonSong },
                { "BossFight", Content.Load<Song>(@"Music\BossFight") },
                { "Home", Content.Load<Song>(@"Music\Home") },
                { "Fanfare", Content.Load<Song>(@"Music\Fanfare") },
                { "Forest", Content.Load<Song>(@"Music\Forest") }
            };

            // Initialise the displayScreens. A little dubious...
            var intro4 = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\introScreen4"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, LevelManager.CurrentLevel} });
            var intro3 = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\introScreen3"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, intro4 } });
            var intro2 = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\introScreen2"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, intro3 } });
            var intro1 = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\introScreen1"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, intro2 } });
            
            // DEBUG: Skip intro screens
            var titleScreen = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\title"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, intro1 } });
            var highScoreScreen = new DisplayScreen("Title", Content.Load<Texture2D>(@"Screens\scores"),
                new Dictionary<Keys, IGameScreen> { { Keys.S, titleScreen} });
            titleScreen.NextStates.Add(Keys.S, highScoreScreen);

            var defeatScreen = new DisplayScreen("Defeat", 
                Content.Load<Texture2D>(@"Screens\defeat"));
            var victoryFinal = new DisplayScreen("Victory", 
                Content.Load<Texture2D>(@"Screens\victory"),
                new Dictionary<Keys, IGameScreen> { { Keys.S, highScoreScreen},
                    { Keys.Enter, LevelManager.CurrentLevel } });
            var victory1 = new DisplayScreen("Victory",
                Content.Load<Texture2D>(@"Screens\victoryScreen1"),
                new Dictionary<Keys, IGameScreen> { { Keys.Enter, victoryFinal } });
            
            var helpScreen = new OverlayScreen(
                new Sprite(Content.Load<Texture2D>(@"Screens\help"), 2),
                new Vector2(64, 50), Keys.H);

            // Initialise the screenManager
            ScreenManager.Init(titleScreen, intro1, intro2, intro3, intro4, defeatScreen,
                victory1, victoryFinal, highScoreScreen, helpScreen, titleScreen,
                new Sprite(Content.Load<Texture2D>(@"Screens\dialogBackground"), 2),
                Content.Load<Texture2D>("fontAtlas"),
                Content.Load<SpriteFont>("Consolas"),
                songs, Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"),
                Content.Load<SpriteFont>("Vinque20"));

            // Initialise the high scores
            using (var reader = new StreamReader("HighScores.txt"))
            {
                string[] temp = reader.ReadToEnd().Split('\n');
                int money = int.Parse(temp[0]);
                int monsters = int.Parse(temp[1]);
                int time = int.Parse(temp[2]);
                ((DisplayScreen)ScreenManager.HighScoreScreen).DisplayText =
                    $"Most Money Collected: {money}\n" +
                    $"Most Enemies Killed: {monsters}\n" +
                    $"Shortest Completion Time: {time} seconds";
                ScreenManager.HighScoreMoney = money;
                ScreenManager.HighScoreMonsters = monsters;
                ScreenManager.HighScoreTime = time;
            }
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Write out the high score table
                using (var writer = new StreamWriter("HighScores.txt"))
                {
                    writer.WriteLine(ScreenManager.HighScoreMoney);
                    writer.WriteLine(ScreenManager.HighScoreMonsters);
                    writer.WriteLine(ScreenManager.HighScoreTime);
                }
                Exit();
            }   

            var ks = Keyboard.GetState();

            ScreenManager.Update(gameTime, ks);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            ScreenManager.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        private void LoadHero()
        {
            var heroSideTexture = Content.Load<Texture2D>(@"Hero\hero-walk-side");
            var heroFrontTexture = Content.Load<Texture2D>(@"Hero\hero-walk-front");
            var heroBackTexture = Content.Load<Texture2D>(@"Hero\hero-back-walk");

            var heroLeftSprite = new Sprite(heroSideTexture, 1, 6, 2, SpriteEffects.FlipHorizontally);
            var heroRightSprite = new Sprite(heroSideTexture, 1, 6, 2, SpriteEffects.None);
            var heroUpSprite = new Sprite(heroBackTexture, 1, 6, 2, SpriteEffects.None);
            var heroDownSprite = new Sprite(heroFrontTexture, 1, 6, 2, SpriteEffects.None);

            var heroSideIdle = Content.Load<Texture2D>(@"Hero\hero-idle-side");
            var heroDownIdle = Content.Load<Texture2D>(@"Hero\hero-idle-front");
            var heroUpIdle = Content.Load<Texture2D>(@"Hero\hero-idle-back");

            var heroLeftIdleSprite = new Sprite(heroSideIdle, 1, 1, 2, SpriteEffects.FlipHorizontally);
            var heroRightIdleSprite = new Sprite(heroSideIdle, 1, 1, 2, SpriteEffects.None);
            var heroDownIdleSprite = new Sprite(heroDownIdle, 1, 1, 2, SpriteEffects.None);
            var heroUpIdleSprite = new Sprite(heroUpIdle, 1, 1, 2, SpriteEffects.None);

            var upPunch = Content.Load<Texture2D>(@"Hero\hero-attack-back");
            var downPunch = Content.Load<Texture2D>(@"Hero\hero-attack-front");
            var sidePunch = Content.Load<Texture2D>(@"Hero\hero-attack-side");

            var leftPunchSprite = new Sprite(sidePunch, 1, 3, 2, SpriteEffects.FlipHorizontally);
            var rightPunchSprite = new Sprite(sidePunch, 1, 3, 2, SpriteEffects.None);
            var upPunchSprite = new Sprite(upPunch, 1, 3, 2, SpriteEffects.None);
            var downPunchSprite = new Sprite(downPunch, 1, 3, 2, SpriteEffects.None);

            var swordSprite = new Sprite(Content.Load<Texture2D>(@"Hero\sword"), 1, 1, 2, SpriteEffects.None);

            var attackSound = Content.Load<SoundEffect>(@"SoundEffects\swish1");
            var hurtSound = Content.Load<SoundEffect>(@"SoundEffects\playerHit");

            HitScript onHit = (d, enemyBounds) =>
            {
                var player = d as Player;
                if (player.MovementStrategy is RecoilMovementStrategy) return;
                else
                {
                    player.HurtSound.Play();

                    player.PreviousMovementStrategy = player.MovementStrategy;
                    player.PreviousSpeed = player.Speed;
                    float angle = MathHelper.ToDegrees(
                        (float)Math.Atan2(player.Bounds.Y - enemyBounds.Y, 
                        player.Bounds.X - enemyBounds.X));
                    if (angle < 0) angle += 360;

                    /* We don't want the player to get stuck in a corner */
                    
                    player.MovementStrategy = new RecoilMovementStrategy(
                        angle, 500, 200, player, false);

                    if (player.Health > 0) player.Health--;
                }
            };

            hero = new Player(new Vector2(350, 250), 150f, heroLeftSprite, heroRightSprite, heroUpSprite, heroDownSprite,
                heroLeftIdleSprite, heroRightIdleSprite, heroUpIdleSprite, heroDownIdleSprite,
                leftPunchSprite, rightPunchSprite, upPunchSprite, downPunchSprite, swordSprite,
                new Rectangle(11, 23, 10, 7), 7.5f, 0.25f, whiteTexture, attackSound, hurtSound, 
                onHit,
                player => { }, player => { }
                );

            // Scale the hero's bounding box
            hero.Bounds = new Rectangle(
                hero.Bounds.X * 2,
                hero.Bounds.Y * 2,
                hero.Bounds.Width * 2,
                hero.Bounds.Height * 2
                );
        }

        private void LoadPropTemplates()
        {
            // Function templates. These should probably be stored elsewhere...

            HitScript destructiblePropHit = (d, enemyPos) =>
            {
                var dp = d as DestructibleProp;
                if (dp.Sprite != dp.DestroyingSprite)
                {
                    dp.DestructionSound.Play();
                    dp.OnStartDestroy(dp);
                }
            };

            DestructibleScript destructiblePropStartDestroy = d =>
            {
                var dp = d as DestructibleProp;
                LevelManager.RemoveFromCollidables(dp);
                dp.Sprite = dp.DestroyingSprite;
            };

            DestructibleScript destructiblePropEndDestroy = d =>
            {
                var dp = d as DestructibleProp;
                LevelManager.DropLoot(dp);
                LevelManager.MarkDestructibleForRemoval(dp);
            };

            propTemplates = new Dictionary<string, Prop>();

            /*   IMPORTANT NOTES:
             *      1. Position is always Vector2.Zero
             *      2. Bounding boxes are in texture pixels
             */

            var bush = new DestructibleProp(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\bush"),
                2, SpriteEffects.None), new Rectangle(1, 1, 14, 14), true,
                destructiblePropHit,
                destructiblePropStartDestroy,
                destructiblePropEndDestroy,
                new Sprite(Content.Load<Texture2D>(@"Animations\bushDestruction"), 1, 4, 2,
                SpriteEffects.None), 15, 
                Content.Load<SoundEffect>(@"SoundEffects\bushDestruction"));
            propTemplates.Add("bush", bush);

            var plantRed = new DestructibleProp(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\plantRed"),
                2, SpriteEffects.None), new Rectangle(1, 1, 14, 14), true,
                destructiblePropHit,
                destructiblePropStartDestroy,
                destructiblePropEndDestroy,
                new Sprite(Content.Load<Texture2D>(@"Animations\bushDestruction"), 1, 4, 2,
                SpriteEffects.None), 15,
                Content.Load<SoundEffect>(@"SoundEffects\bushDestruction"));
            propTemplates.Add("plantRed", plantRed);

            var plantBlue = new DestructibleProp(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\plantBlue"),
                2, SpriteEffects.None), new Rectangle(1, 1, 14, 14), true,
                destructiblePropHit,
                destructiblePropStartDestroy,
                destructiblePropEndDestroy,
                new Sprite(Content.Load<Texture2D>(@"Animations\bushDestruction"), 1, 4, 2,
                SpriteEffects.None), 15,
                Content.Load<SoundEffect>(@"SoundEffects\bushDestruction"));
            propTemplates.Add("plantBlue", plantBlue);

            var pot = new DestructibleProp(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\pot"),
                2, SpriteEffects.None), new Rectangle(2, 8, 12, 7), true,
                destructiblePropHit,
                destructiblePropStartDestroy,
                destructiblePropEndDestroy,
                new Sprite(Content.Load<Texture2D>(@"Animations\potDestruction"), 
                1, 7, 2, SpriteEffects.None),
                15,
                Content.Load<SoundEffect>(@"SoundEffects\potDestruction"));
            propTemplates.Add("pot", pot);

            var bigRock = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\bigRock"),
                2, SpriteEffects.None), new Rectangle(4, 15, 24, 10), true);
            propTemplates.Add("bigRock", bigRock);

            var smallRock1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallRock1"), 2),
                new Rectangle(3, 4, 10, 7), true);
            propTemplates.Add("smallRock1", smallRock1);

            var smallRock2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallRock2"), 2),
                new Rectangle(1, 3, 14, 10), true);
            propTemplates.Add("smallRock2", smallRock2);

            var smallRock3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallRock3"), 2),
                new Rectangle(1, 4, 13, 11), true);
            propTemplates.Add("smallRock3", smallRock3);

            var smallRock4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallRock4"), 2),
                new Rectangle(1, 3, 14, 10), true);
            propTemplates.Add("smallRock4", smallRock4);

            var smallRock5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallRock5"), 2),
                new Rectangle(2, 8, 12, 6), true);
            propTemplates.Add("smallRock5", smallRock5);

            var flag = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\flag"),
                1, 5, 2, SpriteEffects.None), new Rectangle(1, 52, 6, 6), true, 5);
            propTemplates.Add("flag", flag);

            var smallPine = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\smallPine"),
                2, SpriteEffects.None), new Rectangle(9, 29, 28, 16), true);
            propTemplates.Add("smallPine", smallPine);

            var smallPineEdge1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge1"), 2),
                new Rectangle(9, 29, 28, 16), true);
            propTemplates.Add("smallPineEdge1", smallPineEdge1);

            var smallPineEdge2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge2"), 2),
                new Rectangle(0, 29, 20, 15), true);
            propTemplates.Add("smallPineEdge2", smallPineEdge2);

            var smallPineEdge3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge3"), 2),
                new Rectangle(9, 31, 7, 10), true);
            propTemplates.Add("smallPineEdge3", smallPineEdge3);

            var smallPineEdge4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge4"), 2),
                new Rectangle(11, 27, 21, 17), true);
            propTemplates.Add("smallPineEdge4", smallPineEdge4);

            var smallPineEdge5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge5"), 2),
                new Rectangle(0, 0, 48, 48), true);
            propTemplates.Add("smallPineEdge5", smallPineEdge5);

            var smallPineEdge6 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge6"), 2),
                new Rectangle(0, 0, 32, 48), true);
            propTemplates.Add("smallPineEdge6", smallPineEdge6);

            var smallPineEdge7 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge7"), 2),
                new Rectangle(6, 0, 10, 25), true);
            propTemplates.Add("smallPineEdge7", smallPineEdge7);

            var smallPineEdge8 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge8"), 2),
                new Rectangle(0, 0, 10, 25), true);
            propTemplates.Add("smallPineEdge8", smallPineEdge8);

            var smallPineEdge9 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge9"), 2),
                new Rectangle(0, 30, 7, 12), true);
            propTemplates.Add("smallPineEdge9", smallPineEdge9);

            var smallPineEdge10 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge10"), 2),
                new Rectangle(0, 0, 32, 32), true);
            propTemplates.Add("smallPineEdge10", smallPineEdge10);

            var smallPineEdge11 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge11"), 2),
                new Rectangle(0, 0, 32, 48), true);
            propTemplates.Add("smallPineEdge11", smallPineEdge11);

            var smallPineEdge12 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge12"), 2),
                new Rectangle(0, 0, 32, 48), true);
            propTemplates.Add("smallPineEdge12", smallPineEdge12);

            var smallPineEdge13 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge13"), 2),
                new Rectangle(10, 11, 22, 17), true);
            propTemplates.Add("smallPineEdge13", smallPineEdge13);

            var smallPineEdge14 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\smallPineEdge14"), 2),
                new Rectangle(6, 5, 33, 11), true);
            propTemplates.Add("smallPineEdge14", smallPineEdge14);

            var pineWall1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall1"), 2),
                new Rectangle(0, 0, 80, 48), true);
            propTemplates.Add("pineWall1", pineWall1);

            var pineWall2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall2"), 2),
                new Rectangle(0, 0, 48, 48), true);
            propTemplates.Add("pineWall2", pineWall2);

            var pineWall3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall3"), 2),
                new Rectangle(0, 0, 80, 48), true);
            propTemplates.Add("pineWall3", pineWall3);

            var pineWall4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall4"), 2),
                new Rectangle(0, 0, 121, 44), true);
            propTemplates.Add("pineWall4", pineWall4);

            var pineWall5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall5"), 2),
                new Rectangle(12, 0, 121, 44), true);
            propTemplates.Add("pineWall5", pineWall5);

            var pineWall6 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall6"), 2),
                new Rectangle(0, 0, 48, 48), true);
            propTemplates.Add("pineWall6", pineWall6);

            var pineWall7 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall7"), 2),
                new Rectangle(0, 0, 48, 48), true);
            propTemplates.Add("pineWall7", pineWall7);

            var pineWall8 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall8"), 2),
                new Rectangle(0, 0, 80, 48), true);
            propTemplates.Add("pineWall8", pineWall8);

            var pineWall9 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\pineWall9"), 2),
                new Rectangle(0, 0, 16, 48), true);
            propTemplates.Add("pineWall9", pineWall9);

            var house = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\house"),
                2, SpriteEffects.None), new Rectangle(7, 52, 66, 25), true);
            propTemplates.Add("house", house);

            var caveDoor = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\caveDoor"),
                2, SpriteEffects.None), new Rectangle(0, 0, 16, 32), false);
            propTemplates.Add("caveDoor", caveDoor);

            var welcomeMat = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\welcomeMat"),
                2, SpriteEffects.None), new Rectangle(1, 2, 30, 14), false);
            propTemplates.Add("welcomeMat", welcomeMat);

            var window = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\window"),
                2, SpriteEffects.None), new Rectangle(3, 8, 26, 18), false);
            propTemplates.Add("window", window);

            var bigTree = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\bigTree"),
                2, SpriteEffects.None), new Rectangle(20, 74, 25, 18), true);
            propTemplates.Add("bigTree", bigTree);

            var bigTreeDarker = new Prop(Vector2.Zero, new Sprite(Content.Load<Texture2D>(@"Props\bigTreeDarker"),
                2, SpriteEffects.None), new Rectangle(20, 74, 25, 18), true);
            propTemplates.Add("bigTreeDarker", bigTreeDarker);

            var bigCarpet = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bigCarpet"), 2),
                new Rectangle(-16, -16, 0, 0), false);
            propTemplates.Add("bigCarpet", bigCarpet);

            var bookcase = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bookcase"), 2),
                new Rectangle(1, 30, 46, 14), true);
            propTemplates.Add("bookcase", bookcase);

            var chair1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\chair1"), 2),
                new Rectangle(1, 18, 14, 12), true);
            propTemplates.Add("chair1", chair1);

            var crockeryCupboard = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\crockeryCupboard"), 2),
                new Rectangle(2, 29, 44, 14), true);
            propTemplates.Add("crockeryCupboard", crockeryCupboard);

            var deskBehind = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\deskBehind"), 2),
                new Rectangle(0, 0, 48, 16), true);
            propTemplates.Add("deskBehind", deskBehind);

            var diningTable = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\diningTable"), 2),
                new Rectangle(8, 16, 63, 55), true);
            propTemplates.Add("diningTable", diningTable);

            var painting1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\painting1"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("painting1", painting1);

            var painting2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\painting2"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("painting2", painting2);

            var painting3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\painting3"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("painting3", painting3);

            var painting4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\painting4"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("painting4", painting4);

            var painting5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\painting5"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("painting5", painting5);

            var potPlantBig = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\potPlantBig"), 2),
                new Rectangle(2, 23, 12, 7), true);
            propTemplates.Add("potPlantBig", potPlantBig);

            var potPlantSmall = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\potPlantSmall"), 2),
                new Rectangle(3, 10, 10, 5), true);
            propTemplates.Add("potPlantSmall", potPlantSmall);

            var singleBedYellow = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\singleBedYellow"), 2),
                new Rectangle(0, 7, 16, 38), true);
            propTemplates.Add("singleBedYellow", singleBedYellow);

            /***** Cave *****/

            var bones1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bones1"), 2),
                new Rectangle(3, 4, 9, 10), true);
            propTemplates.Add("bones1", bones1);

            var bones2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bones2"), 2),
                new Rectangle(-16, -16, 0, 0), false);
            propTemplates.Add("bones2", bones2);

            var bones3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bones3"), 2),
                new Rectangle(-16, -16, 0, 0), false);
            propTemplates.Add("bones3", bones3);

            var bones4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bones4"), 2),
                new Rectangle(-16, -16, 0, 0), false);
            propTemplates.Add("bones4", bones4);

            var bones5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bones5"), 2),
                new Rectangle(2, 5, 11, 9), true);
            propTemplates.Add("bones5", bones5);

            // TODO: Randomise animation speed?
            var torch = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\torch"), 1, 2, 2),
                new Rectangle(6, 11, 4, 3), true, 3);
            propTemplates.Add("torch", torch);

            var caveRockBig = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\caveRockBig"), 2),
                new Rectangle(2, 10, 28, 19), true);
            propTemplates.Add("caveRockBig", caveRockBig);

            var caveRockSmall1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\caveRockSmall1"), 2),
                new Rectangle(2, 5, 13, 9), true);
            propTemplates.Add("caveRockSmall1", caveRockSmall1);

            var caveRockSmall2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\caveRockSmall2"), 2),
                new Rectangle(1, 5, 14, 9), true);
            propTemplates.Add("caveRockSmall2", caveRockSmall2);

            var caveRockSmall3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\caveRockSmall3"), 2),
                new Rectangle(1, 3, 14, 9), true);
            propTemplates.Add("caveRockSmall3", caveRockSmall3);

            // Special rock that will be destroyed when the minotaur is killed
            var caveRockBigDestructible = new DestructibleProp(Vector2.Zero, 
                new Sprite(Content.Load<Texture2D>(@"Props\caveRockBig"), 2), 
                new Rectangle(2, 10, 28, 19), true,
                (d, eb) => { },
                destructiblePropStartDestroy,
                d =>
                {
                    var dp = d as DestructibleProp;
                    LevelManager.MarkDestructibleForRemoval(dp);
                },
                new Sprite(Content.Load<Texture2D>(@"Animations\smokeAnim"), 1, 7, 2),
                15,
                Content.Load<SoundEffect>(@"SoundEffects\potDestruction"), "caveRockBigDestructible");
            propTemplates.Add("caveRockBigDestructible", caveRockBigDestructible);

            var treasure1 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\treasure1"), 2),
                new Rectangle(4, 9, 25, 21), true);
            propTemplates.Add("treasure1", treasure1);

            var treasure2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\treasure2"), 2),
                new Rectangle(5, 16, 14, 13), true);
            propTemplates.Add("treasure2", treasure2);

            var treasure3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\treasure3"), 2),
                new Rectangle(8, 24, 12, 16), true);
            propTemplates.Add("treasure3", treasure3);

            var treasure4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\treasure4"), 2),
                new Rectangle(2, 7, 27, 22), true);
            propTemplates.Add("treasure4", treasure4);

            var treasure5 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\treasure5"), 2),
                new Rectangle(9, 13, 14, 12), true);
            propTemplates.Add("treasure5", treasure5);

            /***** Forest *****/

            var bigRock2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bigRock2"), 2),
                new Rectangle(2, 13, 28, 15), true);
            propTemplates.Add("bigRock2", bigRock2);

            var fern = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\fern"), 2),
                new Rectangle(10, 11, 13, 9), true);
            propTemplates.Add("fern", fern);

            var fern2 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\fern2"), 2),
                new Rectangle(10, 11, 13, 9), true);
            propTemplates.Add("fern2", fern2);

            var forestTree3 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\forestTree3"), 2),
                new Rectangle(11, 26, 26, 17), true);
            propTemplates.Add("forestTree3", forestTree3);

            var forestTree4 = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\forestTree4"), 2),
                new Rectangle(7, 31, 18, 15), true);
            propTemplates.Add("forestTree4", forestTree4);

            var fountainForest = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\fountainForest"), 1, 3, 2),
                new Rectangle(0, 5, 48, 41), true, 12);
            propTemplates.Add("fountainForest", fountainForest);

            var grave = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\grave"), 2),
                new Rectangle(7, 15, 16, 9), true);
            propTemplates.Add("grave", grave);

            var mushrooms = new Prop(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\mushrooms"), 2),
                new Rectangle(0, 0, 1, 1), false);
            propTemplates.Add("mushrooms", mushrooms);
        }

        private void LoadMobileTemplates()
        {
            // TODO: Move function templates somewhere a little less cramped...
            HitScript basicMonsterHit = (d, enemyBounds) =>
            {
                var m = d as Monster;
                // Don't do anything to a monster that is already recoiling or exploding
                if (m.MovementStrategy is RecoilMovementStrategy ||
                    m.CurrentSprite == m.DestroyingSprite) return;
                // Save existing speed and movementStrategy
                m.PreviousMovementStrategy = m.MovementStrategy;
                m.PreviousSpeed = m.Speed;
                m.Health -= LevelManager.CurrentLevel.Hero.AttackPower;
                // Recoil direction is determined by the angle between the player and the monster
                float angle = MathHelper.ToDegrees(
                    (float)Math.Atan2(m.Bounds.Y - enemyBounds.Y, m.Bounds.X - enemyBounds.X));
                if (angle < 0) angle += 360;
                m.MovementStrategy = new RecoilMovementStrategy(
                    angle, 500, 200, m, m.Health <= 0);
            };

            HitScript minotaurHit = (d, enemyBounds) =>
            {
                var m = d as Monster;
                // Don't do anything to a monster that is already recoiling or exploding
                if (m.MovementStrategy is RecoilMovementStrategy ||
                    m.MovementStrategy is WaypointMovementStrategy ||
                    m.MovementStrategy is VibrateMovementStrategy ||
                    m.CurrentSprite == m.DestroyingSprite) return;
                // Save existing speed and movementStrategy
                m.PreviousMovementStrategy = m.MovementStrategy;
                m.PreviousSpeed = m.Speed;
                m.Health -= LevelManager.CurrentLevel.Hero.AttackPower;
                // Recoil direction is determined by the angle between the player and the monster
                float angle = MathHelper.ToDegrees(
                    (float)Math.Atan2(m.Bounds.Y - enemyBounds.Y, m.Bounds.X - enemyBounds.X));
                if (angle < 0) angle += 360;
                m.MovementStrategy = new RecoilMovementStrategy(
                    angle, 500, 100, m, m.Health <= 0);
            };

            HitScript noRecoilHit = (d, enemyBounds) =>
            {
                var m = d as Monster;
                if (m.IsInvulnerable) return;
                m.Health -= LevelManager.CurrentLevel.Hero.AttackPower;
                m.IsInvulnerable = true;
                Console.WriteLine(m.Health);
                if (m.Health <= 0) m.OnStartDestroy(m);
            };

            DestructibleScript basicMonsterStartDestroy = d =>
            {
                var m = d as Monster;
                m.IsSolid = false;
                m.Transparency = 1;
                // Position the explosion at the centre of the monster
                // TODO: Copy this to props
                int widthDifference =
                    (m.DestroyingSprite.FrameWidth - m.CurrentSprite.FrameWidth) / 2;
                int heightDifference =
                    (m.DestroyingSprite.FrameHeight - m.CurrentSprite.FrameHeight) / 2;
                m.Position = new Vector2(m.Position.X - widthDifference, m.Position.Y - heightDifference);
                m.CurrentSprite = m.DestroyingSprite;
                m.DestructionSound.Play();
                // Stop monster from moving
                m.MovementStrategy = new StationaryMovementStrategy();
                // Change the frame speed to what was specified for the destruction animation
                m.UpdateInterval = 1000f / m.DestroyingAnimsPerSecond;
            };
            
            DestructibleScript basicMonsterEndDestroy = d =>
            {
                var m = d as Monster;
                LevelManager.DropLoot(m);
                LevelManager.MarkDestructibleForRemoval(m);
                LevelManager.CurrentLevel.Hero.NumEnemiesKilled++;
            };

            DestructibleScript minotaurEndDestroy = d =>
            {
                var m = d as Monster;
                LevelManager.MarkDestructibleForRemoval(m);
                LevelManager.CurrentLevel.destructibles.ForEach(de =>
                {
                    if (de is DestructibleProp)
                    {
                        var dp = de as DestructibleProp;
                        if (dp.TemplateName == "caveRockBigDestructible")
                        {
                            dp.OnStartDestroy(dp);
                        }
                    }
                });

                // Remove the zoneTriggers
                Level level = LevelManager.CurrentZone.First(l => l.X == 0 && l.Y == 2);
                level.zoneTriggers = level.zoneTriggers.Where(zt => zt.Zone != "CrownCave").ToList();
                Console.WriteLine(level.zoneTriggers.Count);

                // Play the normal cave music
                // TODO: Add a 'fade then play' method
                ScreenManager.PlaySong("CrownCave");

                LevelManager.CurrentLevel.Hero.NumEnemiesKilled++;
            };

            // TODO: Do I need to load the destruction texture separately each time? Probably not.

            mobileTemplates = new Dictionary<string, Mobile>();

            var slimeBlue = new Monster(Vector2.Zero, new Rectangle(2, 4, 12, 10), true, 30f,
                8f, new RandomMovementStrategy(), new Sprite(
                    Content.Load<Texture2D>(@"Mobiles\slimeBlue"), 1, 4, 2, SpriteEffects.None),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2, SpriteEffects.None),
                15,
                Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 1,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy);
            mobileTemplates.Add("slimeBlue", slimeBlue);

            var slimeGreen = new Monster(Vector2.Zero, new Rectangle(2, 4, 12, 10), true, 30f,
                8f, new AvoidPlayerMovementStrategy(), new Sprite(
                    Content.Load<Texture2D>(@"Mobiles\slimeGreen"), 1, 4, 2, SpriteEffects.None),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2, SpriteEffects.None),
                15,
                Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 1,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy);
            mobileTemplates.Add("slimeGreen", slimeGreen);

            var slimeOrange = new Monster(Vector2.Zero, new Rectangle(2, 4, 12, 10), true, 60f,
                8f, new FollowPlayerMovementStrategy(), new Sprite(
                    Content.Load<Texture2D>(@"Mobiles\slimeOrange"), 1, 4, 2, SpriteEffects.None),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2, SpriteEffects.None),
                15,
                Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 1,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy);
            mobileTemplates.Add("slimeOrange", slimeOrange);
            
            var skeleton = new Monster(Vector2.Zero, new Rectangle(12, 11, 13, 23), true,
                90f, 8f, new HorizontalMovementStrategy(),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\skeletonEast"), 1, 13, 2),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2), 15,
                Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 1,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy,
                null,
                new Sprite(Content.Load<Texture2D>(@"Mobiles\skeletonEast"), 1, 13, 2),
                null,
                new Sprite(Content.Load<Texture2D>(@"Mobiles\skeletonWest"), 1, 13, 2));
            mobileTemplates.Add("skeleton", skeleton);

            var ghost = new Monster(Vector2.Zero, new Rectangle(8, 23, 21, 7), false,
                60f, 1f, new FollowPlayerGradualMovementStrategy(),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\ghost"), 2),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2),
                15, Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 2,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy, null, null, null,
                null, 0.5f);
            mobileTemplates.Add("ghost", ghost);

            var log = new Monster(Vector2.Zero, new Rectangle(10, 17, 13, 9), true, 45f,
                8f, new CardinalMovementStrategy(),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\logEast"), 1, 4, 2),
                new Sprite(Content.Load<Texture2D>(@"Animations\enemy-death"), 1, 6, 2), 15,
                Content.Load<SoundEffect>(@"SoundEffects\slimeDeath"), 1,
                basicMonsterHit, basicMonsterStartDestroy, basicMonsterEndDestroy,
                new Sprite(Content.Load<Texture2D>(@"Mobiles\logNorth"), 1, 4, 2),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\logEast"), 1, 4, 2),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\logSouth"), 1, 4, 2),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\logWest"), 1, 4, 2));
            mobileTemplates.Add("log", log);
            
            var minotaur = new Monster(Vector2.Zero, new Rectangle(27, 5, 30, 40), true, 60f,
                8f, new FollowPlayerMovementStrategy(),
                new Sprite(Content.Load<Texture2D>(@"Mobiles\minotaurMoveEast"), 1, 8, 2),
                new Sprite(Content.Load<Texture2D>(@"Animations\circleExplosion"), 1, 10, 2), 10,
                Content.Load<SoundEffect>(@"SoundEffects\explosion"), 20,
                minotaurHit, basicMonsterStartDestroy, minotaurEndDestroy,
                null, 
                new Sprite(Content.Load<Texture2D>(@"Mobiles\minotaurMoveEast"), 1, 8, 2), 
                null,
                new Sprite(Content.Load<Texture2D>(@"Mobiles\minotaurMoveWest"), 1, 8, 2),
                1, 2500, 
                monster =>
                {
                    if (monster.MovementStrategy is FollowPlayerMovementStrategy)
                    {
                        monster.TimerLengthMS = 1000;
                        //monster.target = new Vector2(
                        //    LevelManager.CurrentLevel.Hero.Bounds.X,
                        //    LevelManager.CurrentLevel.Hero.Bounds.Y);

                        monster.Speed = 0f;

                        monster.target = LevelManager.CurrentLevel.Hero.Position;
                        monster.PowerUpSoundEffect.Play();
                        monster.MovementStrategy = new VibrateMovementStrategy(1);
                    }
                    else if (monster.MovementStrategy is VibrateMovementStrategy)
                    {
                        monster.TimerLengthMS = 800;
                        monster.Speed = 450f;
                        monster.MovementStrategy = new WaypointMovementStrategy(monster.target);
                    }
                    // TODO: Fix WaypointMovement to prevent minotaur getting stuck?
                    else if (monster.MovementStrategy is WaypointMovementStrategy)
                    {
                        monster.TimerLengthMS = 2500;
                        monster.Speed = 60f;
                        monster.MovementStrategy = new FollowPlayerMovementStrategy();
                    }
                }, true, Content.Load<SoundEffect>(@"SoundEffects\minotaurPowerUp")
                );
            mobileTemplates.Add("minotaur", minotaur);
        }

        private void LoadCollectibleTemplates()
        {
            // TODO: Move function templates
            HitScript collectibleOnHit = (d, enemyBounds) =>
            {
                var c = d as CollectibleProp;
                c.OnCollect(LevelManager.CurrentLevel.Hero, c);
                LevelManager.MarkDestructibleForRemoval(c);
            };

            DestructibleScript collectibleStartDestroy = d => { };

            DestructibleScript collectibleEndDestroy = d => { };

            collectibleTemplates = new Dictionary<string, CollectibleProp>();

            var heart = new CollectibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\heartCollectible"), 1, 4, 2, 
                    SpriteEffects.None),
                new Rectangle(2, 3, 11, 10), false, 12,
                collectibleOnHit, collectibleStartDestroy, collectibleEndDestroy,
                (player, h) =>
                {
                    player.Health += 4;
                    if (player.Health > player.MaxHealth) player.Health = player.MaxHealth;
                    h.collectionSound.Play();
                },
                Content.Load<SoundEffect>(@"SoundEffects\heartCollect"));
            collectibleTemplates.Add("heart", heart);

            var goldCoin = new CollectibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\goldCoinCollectible"), 1, 4, 2,
                    SpriteEffects.None),
                new Rectangle(2, 3, 11, 11), false, 12,
                collectibleOnHit, collectibleStartDestroy, collectibleEndDestroy,
                (player, c) =>
                {
                    player.Money += 10;
                    if (player.Money > player.MaxMoney) player.Money = player.MaxMoney;
                    c.collectionSound.Play();
                },
                Content.Load<SoundEffect>(@"SoundEffects\coinCollect"));
            collectibleTemplates.Add("goldCoin", goldCoin);

            var silverCoin = new CollectibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\silverCoinCollectible"), 1, 4, 2,
                    SpriteEffects.None),
                new Rectangle(2, 3, 11, 11), false, 12,
                collectibleOnHit, collectibleStartDestroy, collectibleEndDestroy,
                (player, c) =>
                {
                    player.Money += 5;
                    if (player.Money > player.MaxMoney) player.Money = player.MaxMoney;
                    c.collectionSound.Play();
                },
                Content.Load<SoundEffect>(@"SoundEffects\coinCollect"));
            collectibleTemplates.Add("silverCoin", silverCoin);

            var bronzeCoin = new CollectibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Props\bronzeCoinCollectible"), 1, 4, 2,
                    SpriteEffects.None),
                new Rectangle(2, 3, 11, 11), false, 12,
                collectibleOnHit, collectibleStartDestroy, collectibleEndDestroy,
                (player, c) =>
                {
                    player.Money += 1;
                    if (player.Money > player.MaxMoney) player.Money = player.MaxMoney;
                    c.collectionSound.Play();
                },
                Content.Load<SoundEffect>(@"SoundEffects\coinCollect"));
            collectibleTemplates.Add("bronzeCoin", bronzeCoin);
        }

        private void LoadInteractibleTemplates()
        {
            /* interactPosition is screen pixels relative to the top-left corner */

            interactibleTemplates = new Dictionary<string, InteractibleProp>();

            var sign = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\sign"), 2, SpriteEffects.None),
                new Rectangle(5, 10, 6, 5), true,
                "nothing",
                s => {
                    s.InteractionSound.Play();
                    ScreenManager.CurrentState = new DialogScreen(
                        ScreenManager.DialogBox, s.DialogText);
                }, 
                new List<Direction> { Direction.South }, Vector2.Zero, 1, 
                Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"));
            interactibleTemplates.Add("sign", sign);

            var signGrass = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\signGrass"), 2, SpriteEffects.None),
                new Rectangle(5, 10, 6, 5), true,
                "nothing",
                s => {
                    s.InteractionSound.Play();
                    ScreenManager.CurrentState = new DialogScreen(
                        ScreenManager.DialogBox, s.DialogText);
                },
                new List<Direction> { Direction.South }, Vector2.Zero, 1,
                Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"));
            interactibleTemplates.Add("signGrass", signGrass);

            // TODO: Add some kind of error beep sound effect?
            var desk = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\desk"), 2),
                new Rectangle(0, 16, 46, 30), true, "blank",
                d =>
                {
                    d.InteractionSound.Play();
                    ScreenManager.CurrentState = new DialogScreen(
                        ScreenManager.DialogBox, d.DialogText);
                },
                new List<Direction> { Direction.South }, new Vector2(32, 20), 1,
                Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"));
            interactibleTemplates.Add("desk", desk);

            // TODO: Add some kind of error beep sound effect?
            //var deskUpgrade = new InteractibleProp(Vector2.Zero,
            //    new Sprite(Content.Load<Texture2D>(@"Interactibles\deskUpgrade"), 2),
            //    new Rectangle(0, 16, 46, 30), true, "blank",
            //    d =>
            //    {
            //        d.InteractionSound.Play();
            //        ScreenManager.CurrentState = new DialogScreen(
            //            ScreenManager.DialogBox, d.DialogText);
            //    },
            //    new List<Direction> { Direction.South }, new Vector2(32, 20), 1,
            //    Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"));
            //interactibleTemplates.Add("deskUpgrade", deskUpgrade);

            var manTest = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\manTest"), 2, SpriteEffects.None),
                new Rectangle(10, 19, 11, 10), true,
                "nothing",
                m => {
                    m.InteractionSound.Play();
                    ScreenManager.CurrentState = new DialogScreen(
                        ScreenManager.DialogBox, m.DialogText);
                },
                new List<Direction> { Direction.West }, Vector2.Zero, 1,
                Content.Load<SoundEffect>(@"SoundEffects\dialogOpen"));
            interactibleTemplates.Add("manTest", manTest);

            var pedestalSword = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\pedestalSword"), 1, 6, 2),
                new Rectangle(3, 6, 26, 25), true, "nothing",
                sp =>
                {
                    // This feels pretty cheap...
                    if (!LevelManager.CurrentLevel.Hero.HasSword)
                    {
                        LevelManager.CurrentLevel.Hero.HasSword = true;
                        ((InteractibleProp)sp).Sprite = new Sprite(
                                Content.Load<Texture2D>(@"Props\pedestal"), 2);
                        ScreenManager.CurrentState = new DialogScreen(
                            ScreenManager.DialogBox, sp.DialogText);
                        ScreenManager.PlayFanfareThenResume("Fanfare");
                    }
                },
                new List<Direction> { Direction.South }, Vector2.Zero, 6);
            interactibleTemplates.Add("pedestalSword", pedestalSword);

            var pedestalCrown = new InteractibleProp(Vector2.Zero,
                new Sprite(Content.Load<Texture2D>(@"Interactibles\pedestalCrown"), 1, 7, 2),
                new Rectangle(3, 6, 26, 25), true, "nothing",
                i =>
                { 
                    if (!LevelManager.CurrentLevel.Hero.HasCrown)
                    {
                        var player = LevelManager.CurrentLevel.Hero;

                        player.HasCrown = true;
                        ((InteractibleProp)i).Sprite = new Sprite(
                                Content.Load<Texture2D>(@"Props\pedestal"), 2);
                        ScreenManager.CurrentState = new DialogScreen(
                            ScreenManager.DialogBox, i.DialogText);
                        ScreenManager.PlayFanfareThenResume("Fanfare");

                        player.NumMSElapsed = player.MSCounter;
                    }
                },
                new List<Direction> { Direction.South }, Vector2.Zero, 6);
            interactibleTemplates.Add("pedestalCrown", pedestalCrown);
        }

        private List<string> GetMapFromFile(string folderName, string fileName)
        {
            var map = new List<string>();
            using (var reader = new StreamReader(
                $@"TestLevels\{folderName}\{fileName}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    map.Add(line);
                }
            }
            return map;
        }

        private void PopulateLevel(string levelFolderName, string zone, int x, int y)
        {
            var tileMap = GetMapFromFile(levelFolderName, "tileMap.txt");
            var worldObjectMap = GetMapFromFile(levelFolderName, "objectMap.txt");
            var mobileMap = GetMapFromFile(levelFolderName, "mobileMap.txt");
            var zoneTriggerMap = GetMapFromFile(levelFolderName, "zoneTriggerMap.txt");
            var interactibleMap = GetMapFromFile(levelFolderName, "interactibleMap.txt");

            // TEST: Change the level's 'lighting'
            var lightColor = Color.White;

                // Initialise the level
                Level level = new Level(tiles, tileMap, tileBoundingBoxes,
                    propTemplates, worldObjectMap, mobileTemplates, mobileMap,
                    zoneTriggerMap, interactibleTemplates, interactibleMap, hero, 
                    new Vector2(x, y), whiteTexture, zone, lightColor);

            //// Add the level to the levelMap
            //levelMap[x][y] = level;

            // If the level's zone doesn't exist yet, create it
            if (!zones.ContainsKey(zone))
            {
                zones.Add(zone, new List<Level>());
            }
            // Add the level to the zone
            zones[zone].Add(level);
        }
    }
}
