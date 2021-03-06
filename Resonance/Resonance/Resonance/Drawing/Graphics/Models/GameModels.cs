using Microsoft.Xna.Framework.Content;


namespace Resonance
{
    class GameModels
    {
        public static readonly int NEURON           = 1;
        public static readonly int BACTERIA         = 2;
        public static readonly int BAD_VIBE         = 3;
        public static readonly int GOOD_VIBE        = 4;
        public static readonly int GROUND           = 5;
        public static readonly int MUSHROOM         = 6;
        public static readonly int SHOCKWAVE        = 7;
        public static readonly int PICKUP           = 8;
        public static readonly int SHIELD_GV        = 9;
        public static readonly int BV_SPAWNER       = 10;
        public static readonly int BV_EXPLOSION     = 11;
        public static readonly int X2               = 12;
        public static readonly int X3               = 13;
        public static readonly int PLUS4            = 14;
        public static readonly int PLUS5            = 15;
        public static readonly int BULLET           = 17;
        public static readonly int BOSS             = 18;
        public static readonly int WALLS            = 19;
        public static readonly int TRAININGWALLS    = 20;
        public static readonly int CHECKPOINT       = 21;
        public static readonly int PICKUPSWALLS     = 22;
        public static readonly int SURVIVALWALLS    = 23;
        public static readonly int CHECKPOINTWALLS  = 24;
        public static readonly int BOSSWALLS        = 25;
        public static readonly int PICKUPORB        = 26;
        public static readonly int RAMP             = 27;
        public static readonly int BOSSBACKTWIG = 28;
        public static readonly int BOSSLEFTBOTTOM = 29;
        public static readonly int BOSSLEFTMIDDLE = 30;
        public static readonly int BOSSLEFTMIDDLEBOTTOM = 31;
        public static readonly int BOSSRIGHTBOTTOM = 32;
        public static readonly int BOSSRIGHTMIDDLE = 33;
        public static readonly int BOSSRIGHTMIDDLEBOTTOM = 34;
        public static readonly int BOSSTOPLEFT = 35;
        public static readonly int BOSSTOPRIGHT = 36;
        public static readonly int BOSSCENTRALORB = 37;
        public static readonly int BOSSCENTRALSPIKE = 38;
        public static readonly int BOSSLEFTORB = 39;
        public static readonly int BOSSLEFTTWIG = 40;
        public static readonly int BOSSRIGHTORB = 41;
        public static readonly int BOSSRIGHTTWIG = 42;
        public static readonly int BOSSBOTTOMLEFTJAW = 43;
        public static readonly int BOSSBOTTOMRIGHTJAW = 44;
        public static readonly int BOSSLEFTJAW = 45;
        public static readonly int BOSSLEFTSTALKBOTTOM = 46;
        public static readonly int BOSSLEFTSTALKTOP = 47;
        public static readonly int BOSSMIDDLESTALKBOTTOM = 48;
        public static readonly int BOSSMIDDLESTALKTOP = 49;
        public static readonly int BOSSRIGHTJAW = 50;
        public static readonly int BOSSRIGHTSTALKBOTTOM = 51;
        public static readonly int BOSSRIGHTSTALKTOP = 52;
        public static readonly int BOSSTOPJAW = 53;
        public static readonly int BOSSTOPLEFTJAW = 54;
        public static readonly int BOSSTOPRIGHTJAW = 55;
        public static readonly int PICKUPDEFLECT    = 56;
        public static readonly int VIRUS = 57;
        public static readonly int TUNNEL = 58;



        private static ContentManager Content;
        private static ImportedGameModels importedGameModels;

        public static TextureAnimation getTextureAnimationInstance(string id)
        {
            return importedGameModels.getTextureAnimationInstance(id);
        }

        public static TextureAnimation getTextureAnimationOriginal(string id)
        {
            return importedGameModels.getTextureAnimationOriginal(id);
        }

        /// <summary>
        /// Creates a new GameModels object and stores all the GameModel objects in one place
        /// to be grabbed by the Drawing class
        /// </summary>
        /// <param name="Content">Pass it the content manager to load textures</param>
        public static void Init(ContentManager newContent)
        {
            Content = newContent;
        }

        /// <summary>
        /// Load all the models for the game
        /// </summary>
        public static ImportedGameModels Load()
        {
            // Edit "Content/Drawing/modelDetails.md" to add new models to the game
            importedGameModels = LoadedContent.getModels(Content);
            return importedGameModels;
        }

        /// <summary>
        /// Returns a GameModel object which contains the Model and scale information
        /// </summary>
        /// <param name="name">Pass it the name of the model e.g GameModels.TREE</param>
        public static GameModel getModel(int name)
        {
            return importedGameModels.getModel(name);
        }
    }
}
