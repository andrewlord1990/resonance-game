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
using AnimationLibrary;


namespace Resonance
{
    class GameModels
    {
        public static readonly int TREE             = 1;
        public static readonly int HOUSE            = 2;
        public static readonly int BAD_VIBE         = 3;
        public static readonly int GOOD_VIBE        = 4;
        public static readonly int GROUND           = 5;
        public static readonly int MUSHROOM         = 6;
        public static readonly int SHOCKWAVE        = 7;
        public static readonly int PICKUP           = 11;//TODO:change back to 8
        public static readonly int SHIELD_GV        = 9;
        public static readonly int BV_SPAWNER       = 10;
        public static readonly int X2               = 11;
        public static readonly int BV_Exploasion    = 12;

        private static ContentManager Content;
        private static ImportedGameModels importedGameModels;

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
        public static void Load()
        {
            // Edit "Content/Drawing/modelDetails.md" to add new models to the game
            importedGameModels = Content.Load<ImportedGameModels>("Drawing/modelDetails");
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
