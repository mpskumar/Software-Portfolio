#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace Gesture
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    /// <remarks>
    /// This class is somewhat similar to one of the same name in the 
    /// GameStateManagement sample.
    /// </remarks>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteBatch spriteBatch;

        /// <summary>
        /// An instance of the HighScoreScreen
        /// </summary>
        HighScoreScreen hss;      

        //public static ConsoleComponent consoleComponent;

        PlayerManager playerManager;
        SpriteManager enemySpriteManager;
        PowerupManager powerupManager;

        ChargeBar chargeBar;
        RepairPowerup repairPowerup;

        BloomComponent bloomComponent;

        // Assume that two-player mode is enabled
        public static bool twoPlayerMode = true;

        //Constants
        const int NUM_ENEMIES = 20;

        /// <summary>
        /// Indicates whether or not the game is over.
        /// </summary>
        bool gameOver;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }


        /// <summary>
        /// Initialize the game, after the ScreenManager is set, but not every time
        /// the graphics are reloaded.
        /// </summary>
        public void Initialize()
        {

            Defines.playersGestures.Clear();
            Defines.playerFusionTriggered.Clear();

            Defines.clientBounds = ScreenManager.Game.Window.ClientBounds;
            //Set up the frames counter
            ScreenManager.Game.Components.Add(new FPSComponent(ScreenManager.Game));

            //Set up the bloom post-process effect
            bloomComponent = new BloomComponent(ScreenManager.Game);
            //bloomComponent.Visible = false;
            ScreenManager.Game.Components.Add(bloomComponent);

            // Initialize an instance of the high score screen
            hss = new HighScoreScreen(ScreenManager.Game);

            //Set up the console writer.
            //consoleComponent = new ConsoleComponent(clientBounds);

            //Set up the sprite managers
            //playerSpriteManager = new SpriteManagerComponent(this);
            enemySpriteManager = new SpriteManager(ScreenManager.Game);

            //Set up the players
            //TODO: Only signed in players should be added.
            playerManager = new PlayerManager(ScreenManager.Game);

            // Check whether if two controllers are currently active (Comment to default to two-player mode)
            // TODO: Add additional check for if the game is currently running or not
            //if (!GamePad.GetState(PlayerIndex.Two).IsConnected)
            //    twoPlayerMode = false;

            // Set up powerup manager
            powerupManager = new PowerupManager(ScreenManager.Game);

            // Set up the charge Bar
            chargeBar = new ChargeBar(ScreenManager.Game);

            // Set up repair powerup
           // repairPowerup = new RepairPowerup(ScreenManager.Game);

            //Set up the particle system manager.
            Defines.particleManager = new ParticleManager(ScreenManager.Game);

            gameOver = false;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //Load up the players based on connected controllers
            for (int i = 0; i < 4; i++)
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                    playerManager.AddPlayer((PlayerIndex)i);

            //If there are no connected controllers
            if (playerManager.playerList.Count == 0)
            {
                //Still add Player 1 -- he'll use the keyboard!
                playerManager.AddPlayer(PlayerIndex.One);
                playerManager.AddPlayer(PlayerIndex.Two);
            }

            //Set the initial positions of the players.
            playerManager.SetInitialPositions();
            //Initialize the system-wide number of players
            Defines.NUM_PLAYERS = playerManager.playerList.Count;
            
            //Load enemies and their initial attributes
            for (int i = 0; i < NUM_ENEMIES; i++)
                //Add the enemy to the sprite manager
                enemySpriteManager.spriteList.Add(new Pawn(ScreenManager.Game));

            //Once the load has finished, we use ResetElapsedTime to tell the game's
            //timing mechanism that we have just finished a very long frame, and that
            //it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            if (spriteBatch != null)
            {
                spriteBatch.Dispose();
                spriteBatch = null;
            }
            
            content.Unload();
            base.UnloadContent();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // if this screen is leaving, then stop the music
            if (IsExiting)
            {
                //audio.StopMusic();
            }
            else if ((otherScreenHasFocus == true) || (coveredByOtherScreen == true))
            {
                // make sure nobody's controller is vibrating
                for (int i = 0; i < 4; i++)
                {
                    GamePad.SetVibration((PlayerIndex)i, 0f, 0f);
                }
                if (gameOver == false)
                {
                  
                }
            }
            else
            {
                // check for a winner
                if (gameOver == false)
                {
                    
                }
                // update the world
                if (gameOver == false)
                {
                    playerManager.Update(gameTime);
                    enemySpriteManager.Update(gameTime);
                    chargeBar.Update(gameTime);
                    //repairPowerup.Update(gameTime);
                    powerupManager.Update(gameTime);

                    //Debug
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed)
                    {
                        //Reset the pawn manager to redisplay all of the pawns
                        foreach (Pawn p in enemySpriteManager.spriteList)
                            p.ResetAttributes();
                    }

                    // Check collision status for players
                    for (int i = 0; i < playerManager.playerList.Count; i++)
                        if (playerManager.playerList[i].isPlayerActive)
                            checkCollision(playerManager.playerList[i]);                    
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
                /*
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                // local players who are participating in the session.
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
            }          
                 */
        }

        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex) || gamePadDisconnected)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
                return false;
            }
            if (input.SeeStats)
            {
                // If they pressed to see their stats, bring up the stats screen.
                List<int> playerScores = new List<int>();
                for (int i = 0; i < playerManager.playerList.Count; i++)
                    playerScores.Add(playerManager.playerList[i].score);

                ScreenManager.AddScreen(new StatsScreen(playerScores));
            }

            // Pressing DPad-Up enables the High Score Screen
            if (input.IsNewButtonPress(Buttons.DPadUp))
                ScreenManager.AddScreen(hss);

            return true;
        }

        private void checkCollision(Player currentPlayer)
        {
            //Check for collisions            
            for (int j = 0; j < currentPlayer.shipManager.shipList.Count; j++)
            {
                //Cache current player
                Gryffon _currentShip = currentPlayer.shipManager.shipList[j];
                for (int i = 0; i < powerupManager.powerupList.Count; i++)
                {
                    // cache powerup
                    AnimatedSprite _powerup = powerupManager.powerupList[i];
                    if (Collision.PlayerWithPowerup(_currentShip, _powerup))
                        PowerupManager.collisionMade(_powerup);

                    PowerupManager.inputManager(_powerup, ref currentPlayer);
                }

                int _playerNumBullets = _currentShip.bulletList.Count;
                for (int i = 0; i < _playerNumBullets; i++)
                {
                    //Cache bullet
                    PlayerBullet _currentBullet = _currentShip.bulletList[i];
                    if (_currentBullet.isActive)
                        for (int k = 0; k < NUM_ENEMIES; k++)
                        {
                            //Cache enemy
                            Pawn _currentPawn = (Pawn)enemySpriteManager.spriteList[k];
                            if (_currentPawn.isAlive && Collision.PlayerBulletWithEnemy(_currentBullet, _currentPawn))
                            {
                                _currentPawn.DecreaseHealth(_currentBullet.damage);
                                _currentBullet.isActive = false;
                                currentPlayer.score += _currentPawn.pointValue;
                                //Pawn is no longer able to take damage
                                _currentPawn.isAlive = false;

                                // Add some charge to the charge bar
                                ChargeBar.addCharge();
                                break;
                            }
                        }
                }
            }

            //Check for collisions between particles and enemies
            List<ParticleSystem> pSystems = Defines.particleManager.GetCollidableParticleSystems();
            for (int i = 0; i < pSystems.Count; i++)
                for (int k = 0; k < NUM_ENEMIES; k++)
                {
                    //Cache enemy
                    Pawn _currentPawn = (Pawn)enemySpriteManager.spriteList[k];

                    //Collision between enemy and particle system
                    if (Collision.EnemyWithParticleSystem(pSystems[i], _currentPawn))
                    {
                        _currentPawn.DecreaseHealth(pSystems[i].damage);
                        currentPlayer.score += _currentPawn.pointValue;
                        //Pawn is no longer able to take damage
                        _currentPawn.isAlive = false;
                        // Add some charge to the charge bar
                        ChargeBar.addCharge();
                    }
                    
                    //Collision between enemy bullets and particle system
                    for (int b = 0; b < _currentPawn.bullets.Count; b++)
                    {
                        EnemyBullet _curBullet = _currentPawn.bullets[b];
                        if(Collision.EnemyWithParticleSystem(pSystems[i], _curBullet))
                        {
                            _curBullet.isActive = false;
                        }
                    }
                }
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            //Draw what's not taken care of by the sprite manager.
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None);

            //playerSpriteManager.Draw(ref spriteBatch, gameTime);
            //player1.Draw(ref spriteBatch, gameTime);
            playerManager.Draw(ref spriteBatch, gameTime);

            enemySpriteManager.Draw(ref spriteBatch, gameTime);

            // Draw the charge bar
            chargeBar.Draw(ref spriteBatch, gameTime);

            // Draw the repair power up
            //repairPowerup.Draw(ref spriteBatch, gameTime);

            // Draw the powerup manager
            powerupManager.Draw(ref spriteBatch, gameTime);

            //Draw particle system information
            Defines.particleManager.Draw(ref spriteBatch, gameTime);
            
            //Print some stats from the enemy manager
            enemySpriteManager.ShowStats(ref spriteBatch);

            spriteBatch.End();
            DrawHud(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        /// <summary>
        /// Draw the user interface elements of the game (scores, etc.).
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        private void DrawHud(GameTime gameTime)
        {
            spriteBatch.Begin();

           

            //Draw game time
            spriteBatch.DrawString(Defines.spriteFontKootenay, "Time", new Vector2(Defines.clientBounds.Width / 2 - 10, 0), Defines.textColor);
            spriteBatch.DrawString(Defines.spriteFontKootenay, gameTime.TotalGameTime.Minutes + ":" + gameTime.TotalGameTime.Seconds.ToString("00"), new Vector2(Defines.clientBounds.Width / 2, 20), Defines.textColor);


           

            //Draw the global gestures and fusio mode flags for each player
            for (int i = 0; i < Defines.playersGestures.Count; i++)
            {
                spriteBatch.DrawString(Defines.spriteFontKootenay, "Player " + (PlayerIndex)i + " Gesture: " + Defines.playersGestures[(PlayerIndex)i].ToString(),
                    new Vector2(Defines.clientBounds.Right - 400, (i+1) * 40), Defines.textColor);

                spriteBatch.DrawString(Defines.spriteFontKootenay, "Player " + (PlayerIndex)i + " Fusion Mode: " + Defines.playerFusionTriggered[(PlayerIndex)i].ToString(),
                   new Vector2(Defines.clientBounds.Right - 400, (i+1) * 60), Defines.textColor);

            }
            

            spriteBatch.End();
        }
        #endregion
    }
}
