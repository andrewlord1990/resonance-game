using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace Resonance
{
    class GameOverScreen : MenuScreen
    {
        GameStats stats;

        Vector2 lPos;
        Vector2 lPosText;
        Vector2 rPos;
        Vector2 rPosText;

        int leftTimes;
        int rightTimes;

        private static System.IAsyncResult result = null;
        private static int ii = 0;

        public GameOverScreen(GameStats stats)
            : base("Game Over")
        {
            this.stats = stats;

            MenuElement playAgain = new MenuElement("Play Again", playGameAgain);
            MenuElement quit = new MenuElement("Back to Main Menu", quitGame);
            MenuElement quitCompletely = new MenuElement("Quit Game", quitGameCompletely);

            MenuItems.Add(playAgain);
            MenuItems.Add(quit);
            MenuItems.Add(quitCompletely);

            
            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);
            lPos = new Vector2(screenSize.X - 580f, screenSize.Y - 550);
            rPos = new Vector2(screenSize.X + 30f, screenSize.Y - 550);

            lPosText = new Vector2(lPos.X + 60f, lPos.Y + 60f);
            rPosText = new Vector2(rPos.X + 60f, rPos.Y + 120f);

            leftTimes = 0;
            rightTimes = 0;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Bgs.Add(this.ScreenManager.Content.Load<Texture2D>("Drawing/UI/Menus/Textures/GameOverScreenBox"));
        }

        public override void HandleInput(InputDevices input)
        {
            base.HandleInput(input);

            bool lastLeft = (input.LastKeys.IsKeyDown(Keys.Left) && input.Keys.IsKeyDown(Keys.Left)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.DPadLeft) && input.PlayerOne.IsButtonDown(Buttons.DPadLeft)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft));
            bool lastRight = (input.LastKeys.IsKeyDown(Keys.Right) && input.Keys.IsKeyDown(Keys.Right)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.DPadRight) && input.PlayerOne.IsButtonDown(Buttons.DPadRight)) ||
                                (input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickRight) && input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickRight));

            bool left = (!input.LastKeys.IsKeyDown(Keys.Left) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadLeft) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft)) &&
                      (input.Keys.IsKeyDown(Keys.Left) || input.PlayerOne.IsButtonDown(Buttons.DPadLeft) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickLeft));
            bool right = (!input.LastKeys.IsKeyDown(Keys.Right) && !input.LastPlayerOne.IsButtonDown(Buttons.DPadRight) &&
                       !input.LastPlayerOne.IsButtonDown(Buttons.LeftThumbstickRight)) &&
                      (input.Keys.IsKeyDown(Keys.Right) || input.PlayerOne.IsButtonDown(Buttons.DPadRight) ||
                       input.PlayerOne.IsButtonDown(Buttons.LeftThumbstickRight));

            if (left) moveUp();
            else if (lastLeft)
            {
                leftTimes++;
                if (leftTimes == 25)
                {
                    moveUp();
                    leftTimes = 0;
                }
            }
            if (right) moveDown();
            else if (lastRight)
            {
                rightTimes++;
                if (rightTimes == 25)
                {
                    moveDown();
                    rightTimes = 0;
                }
            }
        }

        protected override void updateItemLocations()
        {
            base.updateItemLocations();

            Vector2 screenSize = new Vector2(ScreenManager.ScreenWidth / 2, ScreenManager.ScreenHeight);

            //Centre in x is 400
            Vector2 position = new Vector2(screenSize.X - 500, ScreenManager.pixelsY(200));
            for (int i = 0; i < MenuItems.Count; i++)
            {
                position.X = position.X + 250;
                MenuItems[i].Position = position;
            }
        }

        protected override void  drawMenu(int index)
        {
            ScreenManager.SpriteBatch.Draw(Bgs[0], lPos, Color.White);
            ScreenManager.SpriteBatch.Draw(Bgs[0], rPos, Color.White);

            if (!Guide.IsVisible && ii < 3)
            {
                if (ii == 0) ii = 1;

                switch (ii)
                {
                    case 1:
                        {
                            if (HighScoreManager.position != -1)
                            {
                                result = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Player Name", "Enter your name:", "", null, null);
                            }
                            ii = 2;
                            break;
                        }
                    case 2:
                        {   
                        
                            if (result.IsCompleted)
                            {
                                HighScoreManager.data.PlayerName[HighScoreManager.position] = Guide.EndShowKeyboardInput(result);
                                ii = 3;
                                HighScoreManager.saveFile();
                            }
                            break;
                        }
                }
                GamerServicesDispatcher.Update();
            }           
            
            string headings = "Final score: \n\n";
            headings += "Total Bad Vibes Killed: \n\n";
            headings += "Highest Multi-Kill: \n\n";
            headings += "Round 1 Killed: \n\n";
            headings += "Round 2 Time: \n\n";
            headings += "Round 3 Damage Taken: \n\n";
            headings += "Round 4 Time: \n\n";
            headings += "Round 5 Time: \n\n";

            string scores = stats.Score + "\n\n" + stats.BVsKilled + "\n\n" + stats.Multikill + "\n\n";

            ScreenManager.SpriteBatch.DrawString(Font, headings, lPosText, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, scores, new Vector2(lPosText.X + 410, lPosText.Y), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

            headings = "";
            scores = "";
            for (int jj = 0; jj < HighScoreManager.data.SIZE; jj++)
            { 
                headings += HighScoreManager.data.PlayerName[jj] + "\n";
                scores += HighScoreManager.data.Score[jj] + "\n";
            }
            ScreenManager.SpriteBatch.DrawString(Font, "HIGHSCORES", new Vector2(rPos.X + 225f, rPos.Y + 60f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, headings, rPosText, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.DrawString(Font, scores, new Vector2(rPosText.X + 410, rPosText.Y), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

        }

        private void playGameAgain()
        {
            if (GameScreen.mode.MODE == GameMode.OBJECTIVES)
            {
                ObjectiveManager.setObjective(ObjectiveManager.DEFAULT_OBJECTIVE);
                ObjectiveManager.loadObjectivesGame(ScreenManager);
            }
            else
            {
                ScreenManager.game = new GameScreen(ScreenManager, 0);
                LoadingScreen.LoadAScreen(ScreenManager, 1, ScreenManager.game);
            }
        }
        private void quitGame()
        {
            ScreenManager.mainMenu.reset();
            LoadingScreen.LoadAScreen(ScreenManager, 0, ScreenManager.mainMenu);
        }

        private void quitGameCompletely()
        {
            string msg = "Are you sure you want to quit the game?\n";
            msg += "(Enter - OK, Escape - Cancel)";

            ScreenManager.addScreen(new PopupScreen(msg, PopupScreen.QUIT_GAME));
        }
    }
}