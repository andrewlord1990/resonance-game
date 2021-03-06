using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class BVSpawnManager
    {
        public const int SPAWN_RADIUS = 10;
        public const int MAX_ACTIVE = 2;
        public const int MAX_BV = 12;

        public const int OBJECTIVE_MAX_BV = 3;
        public const int ARCADE_MAX_BV = 2000;

        private static int spawnerCount;
        private static int bvcount = 0;
        private static List<BVSpawner> spawners;

        //Allocated variables
        private static List<BadVibe> bvPool;

        public static int getSpawnerCount() {
            return spawners.Count;
        }

        public static List<BVSpawner> getSpawners() {
            return spawners;
        }

        public static void allocate()
        {
            spawnerCount = 1;
            spawners = new List<BVSpawner>();
            bvPool = new List<BadVibe>(50);
            for (int i = 0; i < 50; i++)
            {
                BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BV" + i, Vector3.Zero, 0);
                bvPool.Add(bv);
            }
        }

        public static BadVibe getBadVibe()
        {
            BadVibe bv = bvPool[bvPool.Count - 1];
            bvPool.RemoveAt(bvPool.Count - 1);
            return bv;
        }

        private static void addToPool(BadVibe bv)
        {
            bvPool.Add(bv);
        }

        public static void addNewSpawner(Vector3 position)
        {
            int i = 0;
            BVSpawner spawn;

            if ((GameScreen.mode.MODE == GameMode.OBJECTIVES) && (ObjectiveManager.currentObjective() == ObjectiveManager.KILL_ALL_BV)) {
                spawn = new BVSpawner(GameModels.BV_SPAWNER, "BVSpawner" + spawnerCount, position, OBJECTIVE_MAX_BV, SPAWN_RADIUS, MAX_ACTIVE, false);
            } else if ((GameScreen.mode.MODE == GameMode.OBJECTIVES) && (ObjectiveManager.currentObjective() == ObjectiveManager.SURVIVE)) {
                spawn = new BVSpawner(GameModels.BV_SPAWNER, "BVSpawner" + spawnerCount, position, OBJECTIVE_MAX_BV, SPAWN_RADIUS, MAX_ACTIVE, true);
            } else if (GameScreen.mode.MODE == GameMode.ARCADE) {
                spawn = new BVSpawner(GameModels.BV_SPAWNER, "BVSpawner" + spawnerCount, position, ARCADE_MAX_BV, SPAWN_RADIUS, MAX_ACTIVE, false);
            } else {
                spawn = new BVSpawner(GameModels.BV_SPAWNER, "BVSpawner" + spawnerCount, position, MAX_BV, SPAWN_RADIUS, MAX_ACTIVE, false);
            }

            ScreenManager.game.World.addObject(spawn);
            spawners.Add(spawn);

            Random random = new Random();
            while (i < spawn.MaxActive)
            {
                /*int rand = random.Next(234);
                if (rand % 2 == 0)
                {*/
                    //BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                BadVibe bv = getBadVibe();
                bv.setup(spawn.getSpawnCords(), spawners.Count - 1);
                if (!(spawn.spawnerIsObjectiveSpawner())) spawn.addBV(bv); else BVSpawnManager.addToPool(bv);

                /*}
                else
                {
                    Projectile_BV newProjBV = new Projectile_BV(GameModels.PROJECTILE_BV, "BVA" + bvcount, spawn.getSpawnCords(), (spawners.Count - 1));
                    spawn.addBadVibe( newProjBV);
                }*/
                i++;
            }
            spawnerCount++;
        }

        public static BVSpawner randomSpawner() {
            Random r = new Random();
            int i  = r.Next(spawners.Count);
            return spawners[i];
        }

        public static void vibeDied(BadVibe bv)
        {
            Random random = new Random();

            int s = bv.SpawnerIndex;
            BVSpawner spawn = spawners[s];

            spawn.replaceBV(bv, getBadVibe(), s);
            addToPool(bv);
        }

        public static void spawnOneBVFromEachSpawner() {
            for (int i = 0; i < spawners.Count; i++) {
                spawners[i].addBVFromPool();
            }
        }
    }
}
