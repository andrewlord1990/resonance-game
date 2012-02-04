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
using BEPUphysics;
using ResonanceLibrary;
using BEPUphysics.Paths.PathFollowing;
using System.IO;
using AnimationLibrary;
namespace Resonance
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class Game : Microsoft.Xna.Framework.Game
    {

        public const int BEGINNER = 0;
        public const int EASY     = 1;
        public const int MEDIUM   = 2;
        public const int HARD     = 3;
        public const int EXPERT   = 4;
        public const int INSANE   = 5;

        public static int DIFFICULTY = BEGINNER;

        public static bool USE_SPAWNER = false;

        GraphicsDeviceManager graphics;
        MusicHandler musicHandler;

        World world;
        BVSpawnManager spawner;
        public PickupSpawnManager pickupspawner;
        public AnimationPlayer animationPlayer;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Drawing.Init(Content, graphics);
            musicHandler = new MusicHandler(Content);
            if(USE_SPAWNER) spawner = new BVSpawnManager();
            pickupspawner = new PickupSpawnManager();

            initKeyCache();

            UI.init();

            //Allows you to set the resolution of the game (not tested on Xbox yet)
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Window.AllowUserResizing = true;
        }

        private void initKeyCache()
        {
            KeyboardState kbd = Keyboard.GetState();
            GamePadState one = GamePad.GetState(PlayerIndex.One);
            GamePadState two = GamePad.GetState(PlayerIndex.Two);

            DrumManager.lastKbd = kbd;
            DrumManager.lastPad = two;
            GVManager.lastKbd = kbd;
            GVManager.lastPad = one;
            CameraMotionManager.lastKbd = kbd;
            CameraMotionManager.lastPad = one;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            long start = DateTime.Now.Ticks;
            BadVibe.initialiseBank();

            Drawing.loadContent();
            world = new World();

            //When loading a level via MenuActions the load is done in a separate thread and you get a nice loading screen
            MenuActions.loadLevel(1);

            double loadTime = (double)(DateTime.Now.Ticks - start) / 10000000;
            DebugDisplay.update("LOAD TIME(S)", loadTime.ToString());
        }

        /// <summary>
        /// This method is used to load a level. MenuAction calls this method from a separate thread 
        /// and therefore we can have an animated loading screen while you wait.
        /// </summary>
        /// <param name="i">Int number of the level, taken from the level name, i.e Level1.xml</param>
        public void loadLevel(int i)
        {
            string level = "Levels/Level"+i;
            world.readXmlFile(level, Content);

            GVMotionManager.initialised = false;
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
            Drawing.Update(gameTime);

            if (!Loading.IsLoading)
            {
                keyInput();
                if (!UI.Paused)
                {
                    //Update bad vibe positions
                    List<string> deadVibes = processBadVibes();

                    //Break rest layers
                    if (musicHandler.getTrack().nextQuarterBeat()) breakRestLayers();

                    // Update shockwaves
                    getGV().updateWaves();

                    //Check if combat and freeze range
                    getGV().detectCombatAndFreeze();

                    //Update pickups
                    List<Pickup> pickups = world.returnPickups();
                    world.updatePickups(pickups);


                    world.update();
                    
                    musicHandler.Update();
                    removeDeadBadVibes(deadVibes);
                    if(USE_SPAWNER) spawner.update();
                    pickupspawner.update();

                    //Test ray cast
                    World.rayCast(getGV().Body.Position, getGV().Body.OrientationMatrix.Forward, 10f);

                    animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
                    base.Update(gameTime);
                }
            }
        }

        private void keyInput()
        {
            GamePadState playerOne = GamePad.GetState(PlayerIndex.One);
            GamePadState playerTwo = GamePad.GetState(PlayerIndex.Two);
            KeyboardState keyboard = Keyboard.GetState();

            MenuControlManager.input(playerOne, keyboard);
            if (!UI.Paused)
            {
                //Camera
                CameraMotionManager.update(playerOne, keyboard);
                //Player One
                GVManager.input(playerOne, keyboard);
                //Player Two
                DrumManager.input(playerTwo, keyboard);
            }
        }

        /// <summary>
        /// Process all the bad vibes, either move or kill them
        /// </summary>
        /// <returns>the list of dead bad vibes</returns>
        private List<string> processBadVibes()
        {
            List<string> deadVibes = new List<string>();

            foreach (BadVibe bv in world.returnBadVibes())
            {
                if (bv.Dead)
                {
                    deadVibes.Add(bv.returnIdentifier());
                }
                else if (!bv.Dead)
                {
                    bv.Move();
                }
            }
            return deadVibes;
        }

        /// <summary>
        /// Remove all dead bad vibes from the game
        /// </summary>
        /// <param name="deadVibes">the list of dead bad vibes</param>
        private void removeDeadBadVibes(List<string> deadVibes)
        {
            for (int i = 0; i < deadVibes.Count; i++)
            {
                World.removeObject(World.getObject(deadVibes[i]));
                musicHandler.playSound("beast_dying");
                if(USE_SPAWNER) spawner.vibeDied();
            }
        }

        /// <summary>
        /// Break the rest layer on all bad vibes if present
        /// </summary>
        private void breakRestLayers()
        {
            foreach (BadVibe bv in World.returnBadVibes())
            {
                if (!bv.Dead)
                {
                    bv.damage(Shockwave.REST);
                }
            }
        }

        public static double getDistance(Vector3 from, Vector3 to)
        {
            double xDiff = Math.Abs(from.X - to.X);
            double yDiff = Math.Abs(from.Y - to.Y);
            double zDiff = Math.Abs(from.Z - to.Z);
            double distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) + Math.Pow(zDiff, 2));
            return distance;
        }

        public static GoodVibe getGV()
        {
            return (GoodVibe)Program.game.World.getObject("Player");
        }

        public World World
        {
            get
            {
                return world;
            }
        }

        public MusicHandler Music
        {
            get
            {
                return musicHandler;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            Drawing.Draw(gameTime);
        }

    }
}