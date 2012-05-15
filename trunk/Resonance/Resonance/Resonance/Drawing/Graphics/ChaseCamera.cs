using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.MathExtensions;
using BEPUphysics;

namespace Resonance
{
    class ChaseCamera
    {
        Vector3 position;
        Matrix view;
        Entity chaseObject;

        public ChaseCamera(Vector3 startPos)
        {
            position = startPos;
            view = Matrix.CreateLookAt(new Vector3(0, 15, 15), Vector3.Zero, Vector3.Up);
        }

        private Vector3 calcCamera(Vector3 newPosition)
        {
            /*Vector3 rotation = Utility.QuaternionToEuler(chaseObject.Orientation);
            Vector3 position = chaseObject.Position;
            Matrix rot = Matrix.CreateRotationY(rotation.Y);
            return Vector3.Transform(newPosition, rot) + position;*/

            Vector3 cameraPos;

            if (BulletManager.TRACK_BOSS)
            {
                //Vector3 dir = GameScreen.getBoss().Body.Position - chaseObject.Position;
                //float dist = Vector3.Distance(chaseObject.Position, BulletManager.getBullet().Position);
                cameraPos = Matrix3X3.Transform(newPosition, chaseObject.OrientationMatrix);
                cameraPos += BulletManager.getBullet().Position;
            }
            else
            {
                cameraPos = Matrix3X3.Transform(newPosition, chaseObject.BufferedStates.InterpolatedStates.OrientationMatrix);
                cameraPos += chaseObject.BufferedStates.InterpolatedStates.Position;
            }

            float x = cameraPos.X;
            float z = cameraPos.Z;

            bool locked = false;
            if (x + 7 >= World.PLAYABLE_MAP_X/2 || x - 7 <= -World.PLAYABLE_MAP_X/2)
            {
                cameraPos.X = this.position.X;
                locked = true;
            }
            else if (z + 7 >= World.PLAYABLE_MAP_Z/2 || z - 7 <= -World.PLAYABLE_MAP_Z/2)
            {
                cameraPos.Z = this.position.Z;
                locked = true;
            }
            if (locked) return cameraPos;

            Vector3 rayStart = chaseObject.Position;
            rayStart.Y = 3f;

            Vector3 position;
            RayCastResult result;
            if (chaseObject.Space.RayCast(new Ray(rayStart, cameraPos), Vector3.Distance(rayStart,cameraPos), RayCastFilter, out result))
            {
                //position = result.HitData.Location - (0.01f * cameraPos);
                position = cameraPos;
                DebugDisplay.update("TestCast", "Can see something");
            }
            else
            {
                position = cameraPos;
                DebugDisplay.update("TestCast", "Can't see anything");
            }

            /*List<Object> results = ScreenManager.game.World.rayCastObjects(rayStart, cameraPos, 50f, RayCastFilter);
            if (results.Count > 0)
            {
                DebugDisplay.update("TestCast", results[0].returnIdentifier());
            }
            else
            {
                DebugDisplay.update("TestCast", "Can't see anything");
            }*/

            return position;
        }

        /// <summary>
        /// Updates Camera and HUD based of player position
        /// </summary>
        /// <param name="player">The good vibe class</param>
        public void update(Vector3 newPosition, bool lag)
        {
            Vector3 oldPos = position;
            if(lag) position = Vector3.SmoothStep(oldPos, calcCamera(newPosition), 0.2f);
            else position = calcCamera(newPosition);

            if (BulletManager.TRACK_BOSS)
            {
                view = Matrix.CreateLookAt(position, BulletManager.getBullet().Position, Vector3.Up);
            }
            else
            {
                view = Matrix.CreateLookAt(position, chaseObject.Position, Vector3.Up);
            }
        }

        bool RayCastFilter(BroadPhaseEntry entry)
        {
            bool notSelf = (entry != chaseObject.CollisionInformation);
            bool collisionRules = (entry.CollisionRules.Personal <= CollisionRule.Normal);
            bool notGround = (entry != ((StaticObject)ScreenManager.game.World.getObject("Ground")).Body);
            return notSelf && collisionRules && notGround;
        }

        public Matrix View
        {
            get { return view; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Entity ChaseObject
        {
            set { chaseObject = value; }
        }

    }
}
