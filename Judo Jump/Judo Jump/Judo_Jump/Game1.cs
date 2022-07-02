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

namespace Judo_Jump
{
    public enum GameState
    {
        Cutscene1, Cutscene2, Cutscene3,
        StartScreen,
        LevelSelectScreen,
        ControlsScreen1, ControlsScreen2, ControlsScreen3,
        PlayScreen, PauseScreen, LevelEndScreen
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont cutsceneFont, startScreenFont, tempFont, pauseFont, LevelEndFont;
        public static GameState gameState, nextGameState;
        public static KeyboardState oldKB;
        Song backgroundMusic;
        public static Texture2D whiteTexture, levelBackground;
        Texture2D cutscene1Texture, cutscene2Texture1, cutscene2Texture2, cutscene2Texture3, cutscene3Texture,
                  pauseMenuRectangleText,
                  titleImageTexture, playButtonTexture, gameControlsTexture, menuOptionTexture;
        Rectangle screenRectangle,
                  cutsceneRectangle1, cutsceneRectangle2, cutsceneRectangle3,
                  titleImageRectangle, menuOptionRectangle,
                  buttonBackgroundRect, pauseMenuRectangle1, pauseMenuRectangle2;
                //Level finish screen rectangles
        Rectangle totalScoreRect, coinsCollectedRect, timeTakenRect, retryLevelRect, 
                  previousLevelRect, nextLevelRect, quitGameRect;
        Rectangle[] menuBackgroundRects, menuForegroundRects;
        public static List<string> keyNames;
        public static List<Keys> keyInputs;
        Level level;
        Player act;
        string cutsceneText;
        int timer, currentControlsConfig, controlsScreen3Index, levelNumber, 
            totalScore, coinsCollected, playerTimer, endlevelmenumover;
        float opacity, opacityChange, dynamicOpacity;
        bool ExitBool, resume, quit, restart, previous, next, exit, exitGame;
        Camera cam2B;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
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
            var form = (System.Windows.Forms.Form)
            System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            IsMouseVisible = true;
            resume = true;
            ExitBool = false;
            gameState = GameState.Cutscene1;
            oldKB = Keyboard.GetState();
            pauseMenuRectangle1 = new Rectangle(600, 200, 200, 50);
            pauseMenuRectangle2 = new Rectangle(600, 500, 200, 50);
            screenRectangle = new Rectangle(0, 0, 1200, 800);
            cutsceneRectangle1 = new Rectangle(0, 200, 400, 533);
            cutsceneRectangle2 = new Rectangle(400, 250, 400, 400);
            cutsceneRectangle3 = new Rectangle(800, 300, 400, 333);
            titleImageRectangle = new Rectangle(203, 20, 794, 487);
            menuOptionRectangle = new Rectangle(370, 557, 60, 46);
            buttonBackgroundRect = new Rectangle(125, 25, 320, 100);
            menuBackgroundRects = new Rectangle[13];
            menuForegroundRects = new Rectangle[13];
            menuBackgroundRects[0] = new Rectangle(645, 230, 60, 30);
            menuBackgroundRects[1] = new Rectangle(725, 230, 63, 30);
            menuBackgroundRects[2] = new Rectangle(645, 260, 70, 30);
            menuBackgroundRects[3] = new Rectangle(645, 290, 70, 30);
            menuBackgroundRects[4] = new Rectangle(645, 320, 70, 30);
            menuBackgroundRects[5] = new Rectangle(645, 350, 70, 30);
            menuBackgroundRects[6] = new Rectangle(645, 510, 60, 30);
            menuBackgroundRects[7] = new Rectangle(725, 510, 63, 30);
            menuBackgroundRects[8] = new Rectangle(645, 540, 70, 30);
            menuBackgroundRects[9] = new Rectangle(645, 570, 70, 30);
            menuBackgroundRects[10] = new Rectangle(645, 600, 70, 30);
            menuBackgroundRects[11] = new Rectangle(645, 630, 70, 30);
            menuBackgroundRects[12] = new Rectangle(645, 660, 70, 30);

            totalScoreRect = coinsCollectedRect = timeTakenRect = retryLevelRect
            = previousLevelRect = nextLevelRect = quitGameRect = new Rectangle();
            totalScoreRect = new Rectangle(300, 25, 200, 50);
            coinsCollectedRect = new Rectangle(300, 95, 200, 50);
            timeTakenRect = new Rectangle(300, 165, 200, 50);
            retryLevelRect = new Rectangle(300, 235, 270, 50);
            previousLevelRect = new Rectangle(300, 305, 342, 50);
            nextLevelRect = new Rectangle(300, 375, 245, 50);
            quitGameRect = new Rectangle(300, 445, 227, 50);
            for (int i = 0; i < 13; i++)
            {
                menuForegroundRects[i] = new Rectangle(menuBackgroundRects[i].X + 2, menuBackgroundRects[i].Y + 2,
                                                       menuBackgroundRects[i].Width - 4, menuBackgroundRects[i].Height - 4);
            }
            keyNames = new List<string>();
            keyInputs = new List<Keys>();
            keyNames.Add("cycleUpMenu");
            keyNames.Add("cycleDownMenu");
            keyNames.Add("previousMenu");
            keyNames.Add("select");
            keyNames.Add("pause");
            keyNames.Add("quit");
            keyNames.Add("moveLeft");
            keyNames.Add("moveRight");
            keyNames.Add("jump");
            keyNames.Add("punch");
            keyNames.Add("kick");
            keyNames.Add("useSpecialAbility");
            keyNames.Add("cycleThroughSpecialAbilities");
            keyInputs.Add(Keys.Up);
            keyInputs.Add(Keys.Down);
            keyInputs.Add(Keys.Back);
            keyInputs.Add(Keys.Enter);
            keyInputs.Add(Keys.Tab);
            keyInputs.Add(Keys.Escape);
            keyInputs.Add(Keys.A);
            keyInputs.Add(Keys.D);
            keyInputs.Add(Keys.W);
            keyInputs.Add(Keys.P);
            keyInputs.Add(Keys.K);
            keyInputs.Add(Keys.F);
            keyInputs.Add(Keys.Z);
            level = new Level(Services, cam2B, act);
            cutsceneText = "For many centuries, many high-ranked judo fighters from across the globe have tirelessly\n";
            for (int i = 0; i < 72; i++)
            {
                cutsceneText += " ";
            }
            cutsceneText += "competed to become the ultimate judo-master.";
            timer = 0;
            endlevelmenumover = 0;
            opacity = dynamicOpacity = 0;
            controlsScreen3Index = -1;
            currentControlsConfig = 5;
            levelNumber = 1;
            opacityChange = 0.02f;
            totalScore = Player.getTotalScore();

            coinsCollected = Player.getCoinsCollected();

            playerTimer = Player.getPlayerTimer();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            cutscene1Texture = Content.Load<Texture2D>("Cutscenes/Cutscene1");
            cutscene2Texture1 = Content.Load<Texture2D>("Cutscenes/Cutscene2-Part1");
            cutscene2Texture2 = Content.Load<Texture2D>("Cutscenes/Cutscene2-Part2");
            cutscene2Texture3 = Content.Load<Texture2D>("Cutscenes/Cutscene2-Part3");
            cutscene3Texture = Content.Load<Texture2D>("Cutscenes/Cutscene3");
            titleImageTexture = Content.Load<Texture2D>("Start Screen/Title Image");
            playButtonTexture = Content.Load<Texture2D>("Start Screen/Play Button");
            gameControlsTexture = Content.Load<Texture2D>("Start Screen/Game Controls Icon");
            menuOptionTexture = Content.Load<Texture2D>("Start Screen/Right Arrow");
            whiteTexture = Content.Load<Texture2D>("White Pixel");
            pauseMenuRectangleText = Content.Load<Texture2D>("Menu/rectangle");
            levelBackground = Content.Load<Texture2D>("LevelBackground");
            act = new Player(level, playButtonTexture, keyNames, keyInputs);
            backgroundMusic = Content.Load<Song>("Cutscenes/Cutscene Music");
            cutsceneFont = Content.Load<SpriteFont>("CutsceneFont");
            startScreenFont = Content.Load<SpriteFont>("StartScreenFont");
            tempFont = Content.Load<SpriteFont>("TempFont");
            pauseFont = Content.Load<SpriteFont>("Menu/Text");
            LevelEndFont = Content.Load<SpriteFont>("LevelEndScreen/levelEndFont");
            MediaPlayer.Play(backgroundMusic);
            cam2B = new Camera(GraphicsDevice.Viewport);
            //MediaPlayer.IsRepeating = true;
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
            KeyboardState kb = Keyboard.GetState();

            totalScore = Player.getTotalScore();

            coinsCollected = Player.getCoinsCollected();

            playerTimer = Player.getPlayerTimer();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(keyInputs[keyNames.IndexOf("quit")]))
                this.Exit();

            // TODO: Add your update logic here
            if ((int)gameState < 3 && kb.IsKeyDown(Keys.Space))
            {
                timer = 0;
                MediaPlayer.Stop();
                nextGameState = GameState.LevelSelectScreen;
                gameState = GameState.StartScreen;
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("previousMenu")]) &&
                !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("previousMenu")]))
            {
                if ((int)gameState >= 4 && (int)gameState <= 7)
                {
                    timer = 0;
                    dynamicOpacity = 0;
                    opacityChange = 0.02f;
                    gameState = (GameState)3;
                }
            }
            switch (gameState)
            {
                case GameState.Cutscene1:
                    {
                        if (timer < 900)
                        {
                            timer++;
                        }
                        else
                        {
                            timer = 0;
                            cutsceneText = "Throughout this journey, fighters have to advance many stages, " +
                                           "from rigorous training to preliminary rounds to the final Grand\n" +
                                           "Judo Championship.";
                            gameState = GameState.Cutscene2;
                        }
                        if (opacity < 1 && timer <= 100)
                        {
                            opacity += 0.01f;
                        }
                        if (timer >= 800)
                        {
                            opacity -= 0.01f;
                        }
                        break;
                    }
                case GameState.Cutscene2:
                    {
                        if (timer < 1800)
                        {
                            timer++;
                        }
                        else
                        {
                            timer = 0;
                            cutsceneRectangle1 = new Rectangle(0, 0, 1200, 260);
                            cutsceneRectangle2 = new Rectangle(0, 0, 1200, 600);
                            cutsceneText = "Fortunately, you manage to barely qualify among other\nskilled contestants.";
                            gameState = GameState.Cutscene3;
                        }
                        if (timer % 100 == 0 && timer <= 200)
                        {
                            opacity = 0;
                        }
                        if (opacity < 1)
                        {
                            opacity += 0.01f;
                        }
                        if (timer == 900)
                        {
                            cutsceneText = "In this once-in-a-decade prestigious event, only the top 20 " +
                                           "fighters get the opportunity to further compete.";
                        }
                        break;
                    }
                case GameState.Cutscene3:
                    {
                        if (timer < 900)
                        {
                            timer++;
                        }
                        else
                        {
                            timer = 0;
                            MediaPlayer.Stop();
                            nextGameState = GameState.LevelSelectScreen;
                            gameState = GameState.StartScreen;
                        }
                        if (timer % 100 == 0 && timer <= 200)
                        {
                            opacity = 0;
                        }
                        if (opacity < 1 && timer <= 300)
                        {
                            opacity += 0.01f;
                        }
                        if (timer == 450)
                        {
                            cutsceneText = "Will you become the next Ultimate Judo-Master?";
                        }
                        if (timer >= 800)
                        {
                            opacity -= 0.01f;
                        }
                        break;
                    }
                case GameState.StartScreen:
                    {
                        timer++;
                        if (timer % 30 == 0)
                        {
                            opacityChange *= -1;
                        }
                        dynamicOpacity += opacityChange;
                        if (menuOptionRectangle.Y == 557)
                        {
                            nextGameState = GameState.PlayScreen;
                        }
                        else
                        {
                            nextGameState = (GameState)currentControlsConfig;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]) && menuOptionRectangle.Y != 557)
                        {
                            menuOptionRectangle.Y -= 110;
                            nextGameState = GameState.PlayScreen;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) && menuOptionRectangle.Y != 667)
                        {
                            menuOptionRectangle.Y += 110;
                            nextGameState = (GameState)currentControlsConfig;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("select")]) &&
                            !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("select")]))
                        {
                            if (nextGameState == GameState.PlayScreen)
                            {
                                level.LoadLevel(@"Content/Levels/Level" + levelNumber + ".txt", act);
                            }
                            else
                            {
                                timer = 0;
                                dynamicOpacity = 0;
                                opacityChange = 0.02f;
                            }
                            gameState = nextGameState;
                        }
                        break;
                    }
                case GameState.ControlsScreen1:
                    {
                        timer++;
                        if (timer % 30 == 0)
                        {
                            opacityChange *= -1;
                        }
                        dynamicOpacity += opacityChange;
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("select")]))
                        {
                            currentControlsConfig = 5;
                            keyInputs.Clear();
                            keyInputs.Add(Keys.Up);
                            keyInputs.Add(Keys.Down);
                            keyInputs.Add(Keys.Back);
                            keyInputs.Add(Keys.Enter);
                            keyInputs.Add(Keys.Tab);
                            keyInputs.Add(Keys.Escape);
                            keyInputs.Add(Keys.A);
                            keyInputs.Add(Keys.D);
                            keyInputs.Add(Keys.W);
                            keyInputs.Add(Keys.P);
                            keyInputs.Add(Keys.K);
                            keyInputs.Add(Keys.F);
                            keyInputs.Add(Keys.Z);
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) && !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]))
                        {
                            timer = 0;
                            dynamicOpacity = 0;
                            opacityChange = 0.02f;
                            buttonBackgroundRect.X += 370;
                            gameState = GameState.ControlsScreen2;
                        }
                        break;
                    }
                case GameState.ControlsScreen2:
                    {
                        timer++;
                        if (timer % 30 == 0)
                        {
                            opacityChange *= -1;
                        }
                        dynamicOpacity += opacityChange;
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("select")]))
                        {
                            currentControlsConfig = 6;
                            keyInputs.Clear();
                            keyInputs.Add(Keys.W);
                            keyInputs.Add(Keys.S);
                            keyInputs.Add(Keys.Back);
                            keyInputs.Add(Keys.Enter);
                            keyInputs.Add(Keys.Tab);
                            keyInputs.Add(Keys.Escape);
                            keyInputs.Add(Keys.Left);
                            keyInputs.Add(Keys.Right);
                            keyInputs.Add(Keys.Up);
                            keyInputs.Add(Keys.P);
                            keyInputs.Add(Keys.K);
                            keyInputs.Add(Keys.F);
                            keyInputs.Add(Keys.Z);
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) && !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]))
                        {
                            timer = 0;
                            dynamicOpacity = 0;
                            opacityChange = 0.02f;
                            buttonBackgroundRect.X += 370;
                            buttonBackgroundRect.Width = 205;
                            gameState = GameState.ControlsScreen3;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]) && !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]))
                        {
                            timer = 0;
                            dynamicOpacity = 0;
                            opacityChange = 0.02f;
                            buttonBackgroundRect.X -= 370;
                            gameState = GameState.ControlsScreen1;
                        }
                        break;
                    }
                case GameState.ControlsScreen3:
                    {
                        timer++;
                        if (timer % 30 == 0)
                        {
                            opacityChange *= -1;
                        }
                        dynamicOpacity += opacityChange;
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("select")]))
                        {
                            currentControlsConfig = 7;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]) &&
                            !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]))
                        {
                            if (controlsScreen3Index > -1)
                            {
                                controlsScreen3Index--;
                            }
                            else
                            {
                                timer = 0;
                                dynamicOpacity = 0;
                                opacityChange = 0.02f;
                                buttonBackgroundRect.X -= 370;
                                buttonBackgroundRect.Width = 320;
                                gameState = GameState.ControlsScreen2;
                            }
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) &&
                            !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) &&
                            controlsScreen3Index < 12 && currentControlsConfig == 7)
                        {
                            controlsScreen3Index++;
                        }
                        if (controlsScreen3Index >= 0 && !kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]) &&
                            !kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) &&
                            kb.GetPressedKeys().Count() > 0 && !keyInputs.Contains(kb.GetPressedKeys()[0]))
                        {
                            keyInputs[controlsScreen3Index] = kb.GetPressedKeys()[0];
                        }
                        break;
                    }
                case GameState.PlayScreen:
                    {
                        cam2B.Update(gameTime, act, level);
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("pause")]))
                        {
                            gameState = GameState.PauseScreen;
                        }
                        if ((level.exitRect.Intersects(act.Rectangle) && Level.enemies.Count == 0) ||
                            Level.player.health == 0)
                        {
                            gameState = GameState.LevelEndScreen;
                        }
                        level.Update(cam2B, gameTime);
                        act.Update(gameTime, kb);
                        break;
                    }
                case GameState.PauseScreen:
                    {
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]))
                        {
                            resume = true;
                            quit = false;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]))
                        {

                            quit = true;
                            resume = false;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("select")]))
                        {
                            if (resume)
                            {
                                gameState = GameState.PlayScreen;
                            }
                            else
                            {
                                Exit();
                            }
                        }
                        break;
                    }
                case GameState.LevelEndScreen:
                    {
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]) && endlevelmenumover > 0 && !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleUpMenu")]))
                        {
                            endlevelmenumover--;
                        }
                        if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]) && endlevelmenumover < 3 && !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleDownMenu")]))
                        {
                            endlevelmenumover++;
                        }


                        switch (endlevelmenumover)
                        {
                            default:
                                break;
                            case 0:
                                restart = true;
                                resume = false;
                                previous = false;
                                exit = false;
                                break;

                            case 1:
                                restart = false;
                                resume = false;
                                previous = true;
                                exit = false;
                                break;


                            case 2:
                                restart = false;
                                resume = true;
                                previous = false;
                                exit = false;
                                break;


                            case 3:
                                restart = false;
                                resume = false;
                                previous = false;
                                exit = true;
                                break;
                        }

                        if (endlevelmenumover == 0)
                        {
                            if (kb.IsKeyDown(Keys.Enter) && !oldKB.IsKeyDown(Keys.Enter))
                            {
                                level = new Level(Services, cam2B, act);

                                level.LoadLevel(@"Content/Levels/Level" + levelNumber + ".txt", act);
                                act = new Player(level, playButtonTexture, keyNames, keyInputs);
                                gameState = GameState.PlayScreen;

                            }
                        }
                        if (endlevelmenumover == 1)
                        {
                            if (kb.IsKeyDown(Keys.Enter) && !oldKB.IsKeyDown(Keys.Enter))
                            {
                                if (levelNumber > 1)
                                {
                                    levelNumber--;
                                }
                                level = new Level(Services, cam2B,act);

                                level.LoadLevel(@"Content/Levels/Level" + levelNumber + ".txt", act);
                                act = new Player(level, playButtonTexture, keyNames, keyInputs);
                                gameState = GameState.PlayScreen;
                            }
                        }
                        if (endlevelmenumover == 2)
                        {
                            if (kb.IsKeyDown(Keys.Enter) && !oldKB.IsKeyDown(Keys.Enter))
                            {
                                level = new Level(Services, cam2B,act);
                                levelNumber++;
                                level.LoadLevel(@"Content/Levels/Level" + levelNumber + ".txt", act);
                                act = new Player(level, playButtonTexture, keyNames, keyInputs);
                                gameState = GameState.PlayScreen;

                            }
                        }
                        if (endlevelmenumover == 3)
                        {
                            if (kb.IsKeyDown(Keys.Enter) && !oldKB.IsKeyDown(Keys.Enter))
                            {

                                exitGame = true;
                                Exit();

                            }
                        }
                        break;
                    }
            }
            oldKB = kb;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            // TODO: Add your drawing code here
            if (gameState == GameState.PlayScreen)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam2B.Transform);
            }
            else
            {
                spriteBatch.Begin();
            }
            if ((int)gameState >= 5 && (int)gameState <= 7)
            {
                if ((int)gameState != 7)
                {
                    if ((int)gameState == currentControlsConfig)
                    {
                        spriteBatch.Draw(whiteTexture, buttonBackgroundRect, Color.Red);
                    }
                    else
                    {
                        spriteBatch.Draw(whiteTexture, buttonBackgroundRect, Color.Red * dynamicOpacity);
                    }
                }
                spriteBatch.DrawString(startScreenFont, "Menu Navigation", new Vector2(475, 170), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Cycle through menu options", new Vector2(350, 230), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Go to previous menu", new Vector2(350, 260), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Select option", new Vector2(350, 290), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Pause/Options", new Vector2(350, 320), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Quit game", new Vector2(350, 350), Color.Black);
                spriteBatch.DrawString(startScreenFont, "Player Controls", new Vector2(495, 450), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Move left/right", new Vector2(350, 510), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Jump", new Vector2(350, 540), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Punch", new Vector2(350, 570), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Kick", new Vector2(350, 600), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Use special ability", new Vector2(350, 630), Color.Black);
                spriteBatch.DrawString(cutsceneFont, "Cycle through special abilities", new Vector2(350, 660), Color.Black);
            }
            switch (gameState)
            {
                case GameState.Cutscene1:
                    {
                        spriteBatch.Draw(cutscene1Texture, screenRectangle, Color.White * opacity);
                        spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(350, 700), Color.Black * opacity);
                        break;
                    }
                case GameState.Cutscene2:
                    {
                        if (timer < 100)
                        {
                            spriteBatch.Draw(cutscene2Texture1, cutsceneRectangle1, Color.White * opacity);
                            spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 100), Color.Black * opacity);
                        }
                        else if (timer < 200)
                        {
                            spriteBatch.Draw(cutscene2Texture1, cutsceneRectangle1, Color.White);
                            spriteBatch.Draw(cutscene2Texture2, cutsceneRectangle2, Color.White * opacity);
                            spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 100), Color.Black);
                        }
                        else
                        {
                            if (timer < 1700)
                            {
                                spriteBatch.Draw(cutscene2Texture1, cutsceneRectangle1, Color.White);
                                spriteBatch.Draw(cutscene2Texture2, cutsceneRectangle2, Color.White);
                                spriteBatch.Draw(cutscene2Texture3, cutsceneRectangle3, Color.White * opacity);
                                if (timer >= 900 && timer < 1000)
                                {
                                    spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 100), Color.Black * ((timer - 900) / 100.0f));
                                }
                                else
                                {
                                    spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 100), Color.Black);
                                }
                            }
                            else
                            {
                                spriteBatch.Draw(cutscene2Texture1, cutsceneRectangle1, Color.White * ((1800 - timer) / 100.0f));
                                spriteBatch.Draw(cutscene2Texture2, cutsceneRectangle2, Color.White * ((1800 - timer) / 100.0f));
                                spriteBatch.Draw(cutscene2Texture3, cutsceneRectangle3, Color.White * ((1800 - timer) / 100.0f));
                                spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 100), Color.Black * ((1800 - timer) / 100.0f));
                            }
                        }
                        break;
                    }
                case GameState.Cutscene3:
                    {
                        if (timer < 100)
                        {
                            spriteBatch.Draw(cutscene3Texture, screenRectangle, Color.White * opacity);
                            spriteBatch.Draw(whiteTexture, cutsceneRectangle1, Color.White);
                            spriteBatch.Draw(whiteTexture, cutsceneRectangle2, Color.White);
                            spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 40), Color.Black * opacity);
                        }
                        else if (timer < 200)
                        {
                            spriteBatch.Draw(cutscene3Texture, screenRectangle, Color.White);
                            spriteBatch.Draw(whiteTexture, cutsceneRectangle1, Color.White);
                            spriteBatch.Draw(whiteTexture, cutsceneRectangle2, Color.White * (1 - opacity));
                            spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 40), Color.Black);
                        }
                        else
                        {
                            if (timer < 800)
                            {
                                spriteBatch.Draw(cutscene3Texture, screenRectangle, Color.White);
                                spriteBatch.Draw(whiteTexture, cutsceneRectangle1, Color.White * (1 - opacity));
                                if (timer >= 450 && timer < 550)
                                {
                                    spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 40), Color.Black * ((timer - 450) / 100.0f));
                                    spriteBatch.DrawString(tempFont, "?", new Vector2(586, 49), Color.White * ((timer - 450) / 100.0f));
                                }
                                else
                                {
                                    spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 40), Color.Black);
                                    if (timer >= 450)
                                    {
                                        spriteBatch.DrawString(tempFont, "?", new Vector2(586, 49), Color.White);
                                    }
                                }
                            }
                            else
                            {
                                spriteBatch.Draw(cutscene3Texture, screenRectangle, Color.White * opacity);
                                spriteBatch.DrawString(cutsceneFont, cutsceneText, new Vector2(40, 40), Color.Black * opacity);
                                spriteBatch.DrawString(tempFont, "?", new Vector2(586, 49), Color.White * opacity);
                            }
                        }
                        break;
                    }
                case GameState.StartScreen:
                    {
                        spriteBatch.Draw(titleImageTexture, titleImageRectangle, Color.White * (timer / 100.0f));
                        spriteBatch.Draw(playButtonTexture, new Rectangle(450, 530, 100, 100), Color.White * (timer / 100.0f));
                        spriteBatch.Draw(gameControlsTexture, new Rectangle(450, 640, 100, 100), Color.White * (timer / 100.0f));
                        spriteBatch.Draw(menuOptionTexture, menuOptionRectangle, Color.White * dynamicOpacity);
                        spriteBatch.DrawString(startScreenFont, "PLAY", new Vector2(580, 555), Color.Black * (timer / 100.0f));
                        spriteBatch.DrawString(startScreenFont, "CONTROLS", new Vector2(580, 662), Color.Black * (timer / 100.0f));
                        spriteBatch.DrawString(cutsceneFont, "Press ENTER to select.", new Vector2(1000, 750), Color.Black * (timer / 100.0f));
                        break;
                    }
                case GameState.ControlsScreen1:
                    {
                        spriteBatch.Draw(whiteTexture, new Rectangle(135, 35, 300, 80), Color.White);
                        spriteBatch.DrawString(startScreenFont, "Configuration 1", new Vector2(150, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Configuration 2", new Vector2(520, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Custom", new Vector2(900, 50), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Up/Down arrow keys", new Vector2(650, 230), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Backspace", new Vector2(650, 260), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Enter", new Vector2(650, 290), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Tab", new Vector2(650, 320), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Escape", new Vector2(650, 350), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "A/D", new Vector2(650, 510), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "W", new Vector2(650, 540), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "P", new Vector2(650, 570), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "K", new Vector2(650, 600), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "F", new Vector2(650, 630), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Z", new Vector2(650, 660), Color.Black);
                        break;
                    }
                case GameState.ControlsScreen2:
                    {
                        spriteBatch.Draw(whiteTexture, new Rectangle(505, 35, 300, 80), Color.White);
                        spriteBatch.DrawString(startScreenFont, "Configuration 1", new Vector2(150, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Configuration 2", new Vector2(520, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Custom", new Vector2(900, 50), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "W/S", new Vector2(650, 230), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Backspace", new Vector2(650, 260), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Enter", new Vector2(650, 290), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Tab", new Vector2(650, 320), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Escape", new Vector2(650, 350), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Left/Right arrow keys", new Vector2(650, 510), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Up arrow key", new Vector2(650, 540), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "P", new Vector2(650, 570), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "K", new Vector2(650, 600), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "F", new Vector2(650, 630), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "Z", new Vector2(650, 660), Color.Black);
                        break;
                    }
                case GameState.ControlsScreen3:
                    {
                        if (controlsScreen3Index == -1)
                        {
                            if (currentControlsConfig == 7)
                            {
                                spriteBatch.Draw(whiteTexture, buttonBackgroundRect, Color.Red);
                            }
                            else
                            {
                                spriteBatch.Draw(whiteTexture, buttonBackgroundRect, Color.Red * dynamicOpacity);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(whiteTexture, menuBackgroundRects[controlsScreen3Index], Color.Red);
                            spriteBatch.Draw(whiteTexture, menuForegroundRects[controlsScreen3Index], Color.White);
                        }
                        spriteBatch.Draw(whiteTexture, new Rectangle(875, 35, 185, 80), Color.White);
                        spriteBatch.DrawString(startScreenFont, "Configuration 1", new Vector2(150, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Configuration 2", new Vector2(520, 50), Color.Black);
                        spriteBatch.DrawString(startScreenFont, "Custom", new Vector2(900, 50), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("cycleUpMenu")].ToString(), new Vector2(650, 230), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "/", new Vector2(710, 230), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("cycleDownMenu")].ToString(), new Vector2(730, 230), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("previousMenu")].ToString(), new Vector2(650, 260), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("select")].ToString(), new Vector2(650, 290), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("pause")].ToString(), new Vector2(650, 320), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("quit")].ToString(), new Vector2(650, 350), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("moveLeft")].ToString(), new Vector2(650, 510), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, "/", new Vector2(710, 510), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("moveRight")].ToString(), new Vector2(730, 510), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("jump")].ToString(), new Vector2(650, 540), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("punch")].ToString(), new Vector2(650, 570), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("kick")].ToString(), new Vector2(650, 600), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("useSpecialAbility")].ToString(), new Vector2(650, 630), Color.Black);
                        spriteBatch.DrawString(cutsceneFont, keyInputs[keyNames.IndexOf("cycleThroughSpecialAbilities")].ToString(), new Vector2(650, 660), Color.Black);
                        break;
                    }
                case GameState.PlayScreen:
                    {

                        spriteBatch.Draw(levelBackground, new Rectangle((int)-cam2B.Transform.Translation.X, (int)-cam2B.Transform.Translation.Y, 1200, 800), Color.White);
                        level.Draw(spriteBatch, cam2B);
                        act.Draw(spriteBatch);
                        break;
                    }
                case GameState.PauseScreen:
                    {
                        if (resume == true)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(590, 190, 220, 70), Color.Red);
                        }
                        if (quit == true)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(590, 490, 220, 70), Color.Red);
                        }
                        spriteBatch.Draw(pauseMenuRectangleText, pauseMenuRectangle1, Color.Black);
                        spriteBatch.Draw(pauseMenuRectangleText, pauseMenuRectangle2, Color.Black);
                        spriteBatch.DrawString(pauseFont, "Resume Game", new Vector2(620, 220), Color.White);
                        spriteBatch.DrawString(pauseFont, "Quit Game", new Vector2(620, 520), Color.White);
                        break;
                    }
                case GameState.LevelEndScreen:
                    {
                        //Rectangle Names totalScoreRect, coinsCollectedRect, timeTakenRect, retryLevelRect, 
                        //previousLevelRect, nextLevelRect, quitGameRect;

                        spriteBatch.DrawString(LevelEndFont, "Total Score: " + totalScore, new Vector2(totalScoreRect.X, totalScoreRect.Y), Color.Black);
                        spriteBatch.DrawString(LevelEndFont, "Coins Collected: " + coinsCollected, new Vector2(coinsCollectedRect.X, coinsCollectedRect.Y), Color.Black);
                        spriteBatch.DrawString(LevelEndFont, "Time Taken: " + (playerTimer / 60) + " seconds", new Vector2(timeTakenRect.X, timeTakenRect.Y), Color.Black);
                        if (endlevelmenumover == 0)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(retryLevelRect.X - 10, retryLevelRect.Y + 3, retryLevelRect.Width + 10, retryLevelRect.Height + 10), Color.Red);
                        }
                        if (endlevelmenumover == 1)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(previousLevelRect.X - 10, previousLevelRect.Y + 3, previousLevelRect.Width + 10, previousLevelRect.Height + 10), Color.Red);
                        }
                        if (endlevelmenumover == 2)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(nextLevelRect.X - 10, nextLevelRect.Y + 3, nextLevelRect.Width + 10, nextLevelRect.Height + 10), Color.Red);
                        }
                        if (endlevelmenumover == 3)
                        {
                            spriteBatch.Draw(whiteTexture, new Rectangle(quitGameRect.X - 10, quitGameRect.Y + 3, quitGameRect.Width + 10, quitGameRect.Height + 10), Color.Red);
                        }
                        spriteBatch.DrawString(LevelEndFont, "Retry Level", new Vector2(retryLevelRect.X, retryLevelRect.Y), Color.Black);
                        spriteBatch.DrawString(LevelEndFont, "Previous Level", new Vector2(previousLevelRect.X, previousLevelRect.Y), Color.Black);
                        spriteBatch.DrawString(LevelEndFont, "Next Level", new Vector2(nextLevelRect.X, nextLevelRect.Y), Color.Black);
                        spriteBatch.DrawString(LevelEndFont, "Quit Game", new Vector2(quitGameRect.X, quitGameRect.Y), Color.Black);


                        break;
                    }

            }
            if ((int)gameState < 3)
            {
                if (gameTime.TotalGameTime.Seconds % 2 == 0)
                {
                    spriteBatch.DrawString(cutsceneFont, "Press SPACE to skip.", new Vector2(1000, 20),
                        Color.Red * ((gameTime.TotalGameTime.Milliseconds - (gameTime.TotalGameTime.Milliseconds / 1000 * 1000)) / 1000.0f));
                }
                else
                {
                    spriteBatch.DrawString(cutsceneFont, "Press SPACE to skip.", new Vector2(1000, 20),
                        Color.Red * (1 - ((gameTime.TotalGameTime.Milliseconds - (gameTime.TotalGameTime.Milliseconds / 1000 * 1000)) / 1000.0f)));
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}