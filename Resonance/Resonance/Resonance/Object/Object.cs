using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;
using BEPUphysics;

namespace Resonance
{
    class Object : DrawableGameComponent
    {
        private string identifier;
        protected int gameModelNum;
        protected float rotation;
        private Vector3 position;

        public Object(int modelNum, string name, Game game, Vector3 pos) 
            : base(game)
        {
            gameModelNum = modelNum;
            identifier = name;
            rotation = 0f;
            position = pos;
        }

        public string returnIdentifier()
        {
            return identifier;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this is DynamicObject)
            {
                Drawing.Draw(gameModelNum, ((DynamicObject)this).Body.WorldTransform, ((DynamicObject)this).Body.Position, this);
            }
            else if (this is StaticObject)
            {
                Drawing.Draw(gameModelNum, ((StaticObject)this).Body.WorldTransform.Matrix, position, this);
            }
            base.Draw(gameTime);
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }
        }
    }
}