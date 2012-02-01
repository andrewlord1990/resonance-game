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

namespace Resonance
{
    class Drawing
    {
        private const bool FLOOR_REFLECTIONS = false;

        private static GraphicsDeviceManager graphics;
        private static ContentManager Content;
        private static Hud hud;
        private static Graphics gameGraphics;
        private static LoadingScreen loadingScreen;
        private static int frameCounter;
        private static int frameTime;
        private static int currentFrameRate;
        private static float screenWidth;
        private static float screenHeight;
        private static double widthRatio;
        private static double heightRatio;
        private static Vector3 playerPos;
        private static Texture2D ring;
        private static bool drawingReflection = false;
        private static RenderTarget2D mirrorRenderTarget;
        private static Texture2D shinyFloorTexture;
        private static int drawCount = 0;

        public static bool requestRender
        {
            get
            {
                if (FLOOR_REFLECTIONS && drawCount > 2)
                {
                    drawCount = 0;
                    return true;
                }
                return false;
            }
        }

        public static bool DoDisp
        {
            get;set;
        }

        public static int ScreenWidth
        {
            get
            {
                return (int)Math.Round(screenWidth);
            }
        }

        public static int ScreenHeight
        {
            get
            {
                return (int)Math.Round(screenHeight);
            }
        }

        public static double WidthRatio
        {
            get
            {
                return widthRatio;
            }
        }

        public static double HeightRatio
        {
            get
            {
                return heightRatio;
            }
        }

        public static bool DrawingReflection
        {
            get
            {
                return drawingReflection;
            }
        }

        public static Vector3 CameraPosition
        {
            get
            {
                return gameGraphics.CameraPosition;
            }
        }

        public static Vector3 CameraCoords
        {
            get
            {
                return gameGraphics.CameraCoords;
            }
        }

        public static void reset()
        {
            if (gameGraphics != null) gameGraphics.reset();
        }

        public static void drawReflection()
        {
            graphics.GraphicsDevice.SetRenderTarget(mirrorRenderTarget);
            drawingReflection = true;
        }

        public static void drawGame()
        {
            graphics.GraphicsDevice.SetRenderTarget(null);
            shinyFloorTexture = (Texture2D)mirrorRenderTarget;
            //GameModels.getModel(GameModels.GROUND).setTexture(0,shinyFloorTexture);
            drawingReflection = false;
        }

        public static Texture2D flipTexture(Texture2D source, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            Color[] data = new Color[source.Width * source.Height];
            Color[] flippedData = new Color[data.Length];

            source.GetData<Color>(data);

            for (int x = 0; x < source.Width; x++)
                for (int y = 0; y < source.Height; y++)
                {
                    int idx = (horizontal ? source.Width - 1 - x : x) + ((vertical ? source.Height - 1 - y : y) * source.Width);
                    flippedData[x + y * source.Width] = data[idx];
                }

            flipped.SetData<Color>(flippedData);

            return flipped;
        }

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            Content = newContent;
            graphics = newGraphics;
            GameModels.Init(Content);
            gameGraphics = new Graphics(Content, graphics);
            hud = new Hud(Content,graphics, gameGraphics);
            loadingScreen = new LoadingScreen(Content, graphics);
            playerPos = new Vector3(0,0,0);
            DoDisp = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            widthRatio = screenWidth / 1920;
            heightRatio = screenHeight / 1080;
            hud.loadContent();
            GameModels.Load();
            gameGraphics.loadContent(Content, graphics.GraphicsDevice);
            ring = Content.Load<Texture2D>("Drawing/Textures/texRing");

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            mirrorRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, 2048, 2048, true, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth16);

            loadingScreen.loadContent();
        }

        /// <summary>
        /// Used to update any frame animation
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            if (Loading.IsLoading)
            {
                loadingScreen.Update(gameTime);
            }
            else if (!UI.Paused)
            {
                gameGraphics.update(new Vector2(0f, 0f));
            }
        }

        /// <summary>
        /// Add a displacement wave to the ground texture.
        /// </summary>
        /// <param name="position3d"></param>
        public static void addWave(Vector3 position3d)
        {
            Vector2 playerGroundPos = new Vector2();
            float groundWidth = World.MAP_X;
            float groundHeight = World.MAP_Z;
            float xDis = Math.Abs(position3d.X - World.MAP_MIN_X);
            float yDis = Math.Abs(position3d.Z - World.MAP_MIN_Z);
            playerGroundPos.X = (float)Math.Round(Graphics.DISP_WIDTH * (xDis / groundWidth));
            playerGroundPos.Y = (float)Math.Round(Graphics.DISP_WIDTH * (yDis / groundHeight));
            gameGraphics.addWave(playerGroundPos);
        }

        private static void drawRangeIndicator()
        {
            Vector3 pos = new Vector3(Game.getGV().Body.Position.X, 0.2f, Game.getGV().Body.Position.Z);
            Matrix texturePos = Matrix.CreateTranslation(pos);
            Matrix rotation = Matrix.CreateRotationX((float)(Math.PI/2));
            texturePos = Matrix.Multiply(rotation,texturePos);
            gameGraphics.Draw2dTextures(ring, texturePos, 20, 20);
        }


        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            if (Loading.IsLoading)
            {
                loadingScreen.Draw();
            }
            else
            {
                drawCount++;
                /*Vector3 pos = new Vector3(Game.getGV().Body.Position.X, 0.1f, Game.getGV().Body.Position.Z);
                Matrix texturePos = Matrix.CreateTranslation(pos);
                Matrix rotation = Matrix.CreateRotationX((float)(Math.PI/2*3));
                texturePos = Matrix.Multiply(rotation,texturePos);
                gameGraphics.Draw2dTextures(shinyFloorTexture, texturePos, 20, 20);
                */


                drawRangeIndicator();
                hud.Draw();
                hud.drawDebugInfo(DebugDisplay.getString());
                if (UI.Paused) hud.drawMenu(UI.getString());
                checkFrameRate(gameTime);
            }
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            if (!worldObject.returnIdentifier().Equals("Ground") || (worldObject.returnIdentifier().Equals("Ground") && !DrawingReflection))
            {
                bool blend = false;
                Vector2 playerGroundPos = new Vector2(0f, 0f);
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                if (worldObject is GoodVibe)
                {
                    int health = ((GoodVibe)((DynamicObject)worldObject)).Health;
                    int score = ((GoodVibe)((DynamicObject)worldObject)).TotalScore;
                    int nitro = ((GoodVibe)((DynamicObject)worldObject)).Nitro;
                    int shield = ((GoodVibe)((DynamicObject)worldObject)).Shield;
                    int freeze = ((GoodVibe)((DynamicObject)worldObject)).Freeze;


                    DebugDisplay.update("HEALTH", health.ToString());
                    DebugDisplay.update("SCORE", score.ToString());
                    DebugDisplay.update("nitro", nitro.ToString());
                    DebugDisplay.update("shield", shield.ToString());
                    DebugDisplay.update("freeze", freeze.ToString());

                    hud.updateGoodVibe(health, score, nitro, shield, freeze);
                    playerPos = ((GoodVibe)((DynamicObject)worldObject)).Body.Position;
                }

                if (DoDisp && worldObject.returnIdentifier().Equals("Ground"))
                {
                    blend = true;
                }

                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                gameGraphics.Draw(worldObject, worldTransform, blend, drawingReflection);

                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                if (worldObject is BadVibe)
                {
                    //Gets list of remaining armour layers
                    List<int> seq = ((BadVibe)worldObject).getLayers();
                    hud.updateEnemy(worldObject.returnIdentifier(), pos, seq);
                }
            }
        }

        /// <summary>
        /// Updates Camera
        /// </summary>
        public static void UpdateCamera(Vector3 point, Vector3 cameraPosition)
        {
            gameGraphics.UpdateCamera(cameraPosition);
        }

        /// <summary>
        /// Converts X pixel values from HD resolution to the current resolution which may not be HD
        /// </summary>
        /// <param name="input">X HD coordinate</param>
        /// <returns>True coordinate</returns>
        public static int pixelsX(int input)
        {
            return (int)Math.Round(input * widthRatio);
        }

        /// <summary>
        /// Converts Y pixel values from HD resolution to the current resolution which may not be HD
        /// </summary>
        /// <param name="input">Y HD coordinate</param>
        /// <returns>True coordinate</returns>
        public static int pixelsY(int input)
        {
            return (int)Math.Round(input * heightRatio);
        }

        /// <summary>
        /// Reset graphic options which may become corrupted by SpriteBatch
        /// </summary>
        public static void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Used to update frame rate information
        /// </summary>
        /// <param name="gameTime"></param>
        private static void checkFrameRate(GameTime gameTime)
        {
            frameCounter++;
            frameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (frameTime >= 1000)
            {
                currentFrameRate = frameCounter;
                frameTime = 0;
                frameCounter = 0;
            }
            DebugDisplay.update("FPS", currentFrameRate.ToString());
        }

    }
}