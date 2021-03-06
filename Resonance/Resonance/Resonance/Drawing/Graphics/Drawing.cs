using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;

namespace Resonance
{
    class Drawing
    {
        private static GraphicsDeviceManager graphics;
        private static ContentManager content;
        private static Hud hud;
        public static Graphics gameGraphics;
        private static int frameCounter;
        private static int frameTime;
        private static int currentFrameRate;
        private static bool drawingReflection = false;
        private static bool drawingShadows = false;
        private static RenderTarget2D mirrorRenderTarget = null;
        private static RenderTarget2D shadowsRenderTarget = null;
        private static Texture2D shinyFloorTexture;
        private static Texture2D shadowsTexture;
        static Texture2D sampleTexture;
        private static bool blend = false;
        //static TextureEffect te;

        /// <summary>
        /// Change the ambient light level of the scene, use positive values to make it brighter, negative values to make it darker.  
        /// </summary>
        /// <param name="light">Vector3 of the additions that should be made to the RGB of the ambient light</param>
        public static void setAmbientLight(Vector3 light)
        {
            gameGraphics.CustomShaders.setAmbientLight(light);
        }

        /// <summary>
        /// Change the saturation level of the scene, the nearer to 1 the channel is the more of that colour will show up e.g Vector3(1,0,0) will show black and white and red colours only.  
        /// </summary>
        /// <param name="light">Vector3 of the additions that should be made to the RGB of the saturation</param>
        public static void setSaturation(Vector3 saturation)
        {
            gameGraphics.CustomShaders.setSaturation(saturation);
        }

        public static ContentManager Content
        {
            get { return content; }
        }

        public static bool DrawingReflection
        {
            get
            {
                return drawingReflection;
            }
        }

        public static void Clear()
        {
            graphics.GraphicsDevice.Clear(Color.Black);
        }

        public static void reset()
        {
            if (gameGraphics != null) gameGraphics.reset();
        }

        public static void setReflectionsRenderTarget()
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            graphics.GraphicsDevice.SetRenderTarget(mirrorRenderTarget);
            drawingReflection = true;
            gameGraphics.setUpReflectionCamera();
            drawingShadows = false;
        }

        public static void setShadowsRenderTarget()
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            graphics.GraphicsDevice.SetRenderTarget(shadowsRenderTarget);
            drawingShadows = true;
            gameGraphics.setUpShadowCamera();
            drawingReflection = false;
        }

        public static void renderShadows(GameTime gameTime)
        {

            Drawing.setShadowsRenderTarget();
            DrawableManager.DrawShadowedObjects(gameTime);
            Drawing.saveShadowsTexture();

        }

        public static void renderReflections(GameTime gameTime)
        {
            Drawing.setReflectionsRenderTarget();
            DrawableManager.DrawReflectedObjects(gameTime);
            Drawing.saveReflectionTexture();
        }

        public static void saveReflectionTexture()
        {
            graphics.GraphicsDevice.SetRenderTarget(null);
            shinyFloorTexture = (Texture2D)mirrorRenderTarget;
            //if (mirrorRenderTarget != null) mirrorRenderTarget.Dispose();
            try
            {
                ((GroundShader)gameGraphics.CustomShaders.Ground).setReflectionTexture(shinyFloorTexture);
            }
            catch (Exception)
            {}
            //if (shinyFloorTexture != null && !shinyFloorTexture.IsDisposed) shinyFloorTexture.Dispose();
            drawingReflection = false;
        }

        public static void saveShadowsTexture()
        {
            graphics.GraphicsDevice.SetRenderTarget(null);
            shadowsTexture = (Texture2D)shadowsRenderTarget;
            //if (shadowsRenderTarget != null) shadowsRenderTarget.Dispose();
            try
            {
                ((GroundShader)gameGraphics.CustomShaders.Ground).setShadowTexture(shadowsTexture);
                //te.Texture = shadowsTexture;
            }
            catch (Exception){}
            //if (shadowsTexture != null && !shadowsTexture.IsDisposed) shadowsTexture.Dispose();
            drawingShadows = false;
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

        public static Texture2D scaleTexture(Texture2D src, float sF) {
            return scaleTexture(src, sF, sF);
        }

        public static int wins1 = 0;
        public static int wins2 = 0;
        public static Texture2D scaleTexture(Texture2D src, float sFX, float sFY) {
            int newW = (int) (src.Width  * sFX);
            int newH = (int) (src.Height * sFY);
            Texture2D scaled = new Texture2D(src.GraphicsDevice, newW, newH);
            Color[] data = new Color[src.Width * src.Height];
            Color[] scaledData = new Color[newW * newH];

            src.GetData<Color>(data);

            int idx;
            float srcX = 0f;
            float srcY = 0f;
            float px = 0;
            float py = 0;
            int x, y, i, j;
            float coeffW = 1f / newW;
            float coeffH = 1f / newH;
            Color c;

            // Choose algorithm most appropriate
            if (newW * newH < src.Width * src.Height) {
                for (x = 0; x < newW; x++) {
                    srcX = (int) ((px * src.Width) + 0.5f);
                    py = 0;
                    for (y = 0; y < newH; y++) {
                        srcY = (int) ((py * src.Height) + 0.5f);

                        idx = (int) srcX + (int) (srcY * src.Width);

                        c = data[idx];
                        scaledData[x + y * newW] = c;

                        py += coeffH;
                    }
                    px += coeffW;
                }
            } else {
                coeffW = (float) newW / (float) src.Width;
                coeffH = (float) newH / (float) src.Height;

                float indX   = 0f;
                float indY   = 0f;
                int   iX     = 0;
                int   iY     = 0;
                int   prevIX = 0;
                int   prevIY = 0;

                for (x = 0; x < src.Width; x++) {
                    indX += coeffW;
                    iX = (int) indX;
                    indY = 0f;
                    for (y = 0; y < src.Height; y++) {
                        indY += coeffH;
                        iY = (int) indY;

                        c = data[x + src.Width * y];
                    
                        for (j = prevIY; j < iY; j++) {
                            idx = j * newW;//(int) srcX + (int) (srcY * src.Width);
                            for (i = prevIX; i < iX; i++) {
                                scaledData[idx + i] = c;
                            }
                        }
                        prevIY = iY;
                    }
                    prevIX = iX;
                }
            }

            scaled.SetData<Color>(scaledData);

            return scaled;
        }

        /// <summary>
        /// Create a drawing object, need to pass it the ContentManager and 
        /// GraphicsDeviceManger for it to use
        /// </summary>
        public static void Init(ContentManager newContent, GraphicsDeviceManager newGraphics)
        {
            content = newContent;
            graphics = newGraphics;
            GameModels.Init(content);
            gameGraphics = new Graphics(content, graphics);
            hud = new Hud(content,graphics, gameGraphics);

            //TextureAnimation ta = new TextureAnimation(shinyFloorTexture);
            //TextureEffect te = new TextureEffect(200,200, new Vector3(10,10,10), true, ta);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content needed for drawing the world.
        /// </summary>
        public static void loadContent()
        {
            sampleTexture = content.Load<Texture2D>("Drawing/Textures/texMissing");
            LoadingScreen.CurrentlyLoading = "Hud Content";
            hud.loadContent();
            LoadingScreen.CurrentlyLoading = "Models, Textures and Animations";
            ImportedGameModels importedModels = GameModels.Load();
            LoadingScreen.CurrentlyLoading = "Displacement Maps";
            gameGraphics.loadContent(importedModels,content, graphics.GraphicsDevice);
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            if(mirrorRenderTarget == null) mirrorRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, GraphicsSettings.REFLECTION_TEXTURE_SIZE, GraphicsSettings.REFLECTION_TEXTURE_SIZE, false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            if (shadowsRenderTarget == null) shadowsRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, GraphicsSettings.SHADOWS_TEXTURE_SIZE, GraphicsSettings.SHADOWS_TEXTURE_SIZE, false, graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            //te = new TextureEffect(5, 5, new Vector3(0, 6, 0), true, new TextureAnimation(shadowsTexture));

            gameGraphics.Shaders.setPointLightPos(new Vector3(0, 8, 0));

            Color[] c = {Color.White};
            shadowsTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            shadowsTexture.SetData<Color>(c);
            ((GroundShader)gameGraphics.CustomShaders.Ground).setShadowTexture(shadowsTexture);
        
        }

        /// <summary>
        /// Used to update any frame animation
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            // Fire trail experiment
            /*if (GVMotionManager.BOOSTING)
            {
                Random randomGen = new Random();
                int num = randomGen.Next(3);
                if (num == 1)
                {
                    new FireTextureEffect(1, 1, new Vector3(GameScreen.getGV().Body.Position.X + 1/(randomGen.Next(-3,2)+1f), 0.3f, GameScreen.getGV().Body.Position.Z));
                }
            }*/

            gameGraphics.Shaders.setPointLightPos(new Vector3(GameScreen.getGV().Body.Position.X, GameScreen.getGV().Body.Position.Y+3, GameScreen.getGV().Body.Position.Z));

            gameGraphics.update(groundPos(GameScreen.getGV().Body.Position, false));
        }

        public static Vector2 groundPos(Vector3 position3d, bool one)
        {
            Vector2 playerGroundPos = new Vector2();
            float pad = -2f;
            float groundWidth = World.MAP_X+pad;
            float groundHeight = World.MAP_Z+pad;
            float xDis = Math.Abs(position3d.X - World.MAP_MIN_X + pad/2);
            float yDis = Math.Abs(position3d.Z - World.MAP_MIN_Z + pad/2);
            if (!one)
            {
                playerGroundPos.X = (float)Math.Round(Graphics.DISP_WIDTH * (xDis / groundWidth));
                playerGroundPos.Y = (float)Math.Round(Graphics.DISP_WIDTH * (yDis / groundHeight));
            }
            else
            {
                playerGroundPos.X = 1-(float)(xDis / groundWidth);
                playerGroundPos.Y = (float)(yDis / groundHeight);

            }
            return playerGroundPos;
        }

        /// <summary>
        /// Add a displacement wave to the ground texture.
        /// </summary>
        /// <param name="position3d"></param>
        public static void addWave(Vector3 position3d)
        {
            gameGraphics.addWave(groundPos(position3d, false));
        }

        /// <summary>
        /// Responsible for drawing particles in 3D space.
        /// </summary>
        private static void drawParticles() {
            ParticleEmitterManager.update();
            Vector3 rotation;
            Vector3 pos;
            float size;
            Matrix texturePos, rX, rY, rZ, rR;
            Vector3 gVFwd, gVRdp, angV;
            Quaternion ang;

            List<Emitter> emitters = ParticleEmitterManager.getEmitters();
            for (int i = 0; i < emitters.Count; i++) {
                Emitter e = emitters[i];
                //DebugDisplay.update("P Count", e.getParticles().Count.ToString());
                List<Particle> particles = e.getParticles();
                for (int j = 0; j < particles.Count; j++) {
                    Particle p = particles[j];
                    try {
                        pos = p.getPos();
                        size = p.getSize();
                        texturePos = Matrix.CreateTranslation(pos);

                        if (!(e is Rain)) {
                            rotation = p.getRotation();

                           if (rotation != Vector3.Zero) {
                                rX = Matrix.CreateRotationX(rotation.X);
                                rY = Matrix.CreateRotationY(rotation.Y);
                                rZ = Matrix.CreateRotationZ(rotation.Z);
                                texturePos = Matrix.Multiply(rX, Matrix.Multiply(rY, Matrix.Multiply(rZ, texturePos)));
                            }
                        } else {
                            //camPos = CameraMotionManager.Camera.Position;
                            //camToGV = GameScreen.getGV().Body.Position - camPos;
                            gVFwd = GameScreen.getGV().Body.OrientationMatrix.Forward;
                            gVRdp = p.getPos() - GameScreen.getGV().Body.Position;
                            //gVFwd = camToGV;
                            //gVRdp = camPos;
                            gVFwd.Normalize();
                            gVRdp.Normalize();

                            rotation = Utility.QuaternionToEuler(GameScreen.getGV().Body.Orientation);

                            if (rotation != Vector3.Zero) {
                                rX = Matrix.CreateRotationX(rotation.X);
                                rY = Matrix.CreateRotationY(rotation.Y + 1.75f);
                                rZ = Matrix.CreateRotationZ(rotation.Z);
                                Toolbox.GetQuaternionBetweenNormalizedVectors(ref gVFwd, ref gVRdp, out ang);
                                angV = Utility.QuaternionToEuler(ang);
                                rR = Matrix.CreateRotationY(ang.Y);
                                texturePos = Matrix.Multiply(rX, Matrix.Multiply(rY, Matrix.Multiply(rZ, Matrix.Multiply(rR, texturePos))));
                            }
                        }
                         
                        gameGraphics.drawParticle(e.getPTex(), texturePos, size, size, p.getColour());
                    } catch (Exception) {}
                }
            }
        }

        public static void DrawTexture(Texture2D texture, Matrix position, float width, float height)
        {
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            gameGraphics.drawTexture(texture, position, width, height, drawingReflection);
        }

        /// <summary>
        /// This is called when the character and the HUD should be drawn.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            resetGraphics();
            DrawableManager.Draw(gameTime);
            drawParticles();
            hud.Draw(gameTime);
            if(GameScreen.USE_DEBUG)
            {
                /*if(GameScreen.USE_PROFILER) hud.drawDebugInfo(DebugDisplay.getString()+Profile.DumpAndReset());
                else */hud.drawDebugInfo(DebugDisplay.getString());
                checkFrameRate(gameTime);
            }
        }

        public static void blendOn()
        {
            blend = true;
        }

        public static void blendOff()
        {
            blend = false;
        }

        public static void setShadows()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        public static void unsetShadows()
        {
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        /// <summary>
        /// This is called when you would like to draw an object on screen.
        /// </summary>
        /// <param name="gameModelNum">The game model reference used for the object you want to draw e.g GameModels.BOX </param>
        /// <param name="worldTransform">The world transform for the object you want to draw, use [object body].WorldTransform </param>
        public static void Draw(Matrix worldTransform, Vector3 pos, Object worldObject)
        {
            if (worldObject.ModelInstance.Transparency > 0)
            {
                graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                gameGraphics.Draw(worldObject, worldTransform, blend, drawingReflection, drawingShadows);
            }
        }

        /// <summary>
        /// Reset graphic options which may become corrupted by SpriteBatch
        /// </summary>
        public static void resetGraphics()
        {
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
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
