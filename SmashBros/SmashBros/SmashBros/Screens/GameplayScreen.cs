using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using SmashBros.System;
using FarseerPhysics.DebugViews;
using FarseerPhysics;
using System.Diagnostics;
using SmashBros.Views;

namespace SmashBros
{
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Sprite spiderman;
        World _world;
        Camera2D _camera;
        DebugViewXNA _debugView;


        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {

            _world = new World(new Vector2(0, 10));

            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            if (_camera == null)
            {
                Viewport v = ScreenManager.GraphicsDevice.Viewport;
                _camera = new Camera2D(ScreenManager.GraphicsDevice);
                _camera.Position = new Vector2(v.Width/2, v.Height/2);
            }
            else
            {
                _camera.ResetCamera();
            }

            if (_debugView == null)
            {
                _debugView = new DebugViewXNA(_world);
                _debugView.AppendFlags(DebugViewFlags.DebugPanel);
                _debugView.DefaultShapeColor = Color.Red;
                _debugView.SleepingShapeColor = Color.Green;
                _debugView.LoadContent(ScreenManager.GraphicsDevice, content);
            }

            
            gameFont = content.Load<SpriteFont>("gamefont");
            spiderman = new Sprite(content, "spiderman", 100, 100, 100,100);
            spiderman.BoundRect(_world, 100, 100, BodyType.Static);

            // Simulating longer load time
            Thread.Sleep(1000);

            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen)
            {
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            }
            else
            {
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            }

            if (IsActive)
            {
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                Vector2 targetPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2, 200);
                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);
            }

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            _camera.Update(gameTime);
        }

        /// <summary>
        /// This should be part of the ControllerState class or a derived class.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(ControllerState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    --movement.X;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    ++movement.X;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    --movement.Y;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    ++movement.Y;
                }

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y += thumbstick.Y;

                if (movement.Length() > 1)
                {
                    movement.Normalize();
                }

                playerPosition += movement * 2;
            }
        }

        public override void Draw(GameTime GameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(0, null, null, null, null, null, _camera.View);

            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);
            spriteBatch.DrawString(gameFont, "Insert Gameplay Here", enemyPosition, Color.DarkRed);

            spiderman.Draw(spriteBatch, GameTime);
            spriteBatch.End();

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            Matrix projection = _camera.SimProjection;
            Matrix view = _camera.SimView;

            _debugView.RenderDebugData(ref projection, ref view);
        }

        #endregion
    }
}
