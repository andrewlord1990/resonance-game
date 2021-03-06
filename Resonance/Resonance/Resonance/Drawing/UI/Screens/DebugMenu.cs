using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class DebugMenu : MenuScreen
    {
        private static int genNumber = 0;

        public DebugMenu()
            : base("Debug Menu")
        {
            //Resume
            //GV to full health
            //Reset object positions
            //Add a BV
            //Kill all BVs
            //Exit the Game

            MenuElement resume = new MenuElement("Resume Game", resumeGame);
            MenuElement health = new MenuElement("Restore GV to Full Health", fullHealth);
            MenuElement resetPos = new MenuElement("Reset Object Positions", softReset);
            MenuElement addBV = new MenuElement("Add a BV to the World", addBadVibe);
            MenuElement killBVs = new MenuElement("Kill All the BVs", killBadVibes);
            MenuElement quit = new MenuElement("Quit the Game", quitGame);

            MenuItems.Add(resume);
            MenuItems.Add(health);
            MenuItems.Add(resetPos);
            MenuItems.Add(addBV);
            MenuItems.Add(killBVs);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            int x = 1350;
            int y = 300;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Position = new Vector2(ScreenManager.pixelsX(x), ScreenManager.pixelsY(y));
                y += 75;
            }
        }

        private void resumeGame()
        {
            ExitScreen();
        }

        public void fullHealth()
        {
            GameScreen.getGV().fullHealth();
        }

        public void killBadVibes()
        {
            List<Object> objects = ScreenManager.game.World.returnObjectSubset<BadVibe>();
            for(int i = 0; i < objects.Count; i++)
            {
                ((BadVibe)objects[i]).kill();
            }
            ExitScreen();
        }

        public void softReset()
        {
            ScreenManager.game.World.reset();
        }

        public void addBadVibe()
        {
            BadVibe bv = new BadVibe(GameModels.BAD_VIBE, "BVDebug" + genNumber, new Vector3(0, 1, -2), 0);
            ScreenManager.game.World.addObject(bv);
            genNumber++;
        }

        private void quitGame()
        {
            ScreenManager.Game.Exit();
        }
    }
}
