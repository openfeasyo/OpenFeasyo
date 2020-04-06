/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using System;

namespace OpenFeasyo.GameTools.UI
{

    public class UIEngine
    {
        public static UIEngine Instance { get; private set; }

        private Activity _currentActivity = null;

        public ContentRepository Content { get; set; }

        public GraphicsDevice Device { get; set; }

        public Screen Screen { get; set; }
        public MusicPlayer MusicPlayer { get; set; }

        public event EventHandler<EventArgs> ActivitiesFinished;

        private void OnActivitiesFinished() {
            if(ActivitiesFinished != null) {
                ActivitiesFinished(this, new EventArgs());
            }
        }

        public event EventHandler<TouchCollection> TouchUpdate;
        private void OnTouchUpdate(TouchCollection collection)
        {
            if (TouchUpdate != null)
            {
                TouchUpdate(this, collection);
            }
        }

        public UIEngine(ContentRepository contentRepo, GraphicsDevice device) {
            Content = contentRepo;
            Device = device;
            MusicPlayer = new MusicPlayer();
            
            Screen = new Screen(device.Viewport.Width, device.Viewport.Height, /* 960 */ (int)(((float)720 / (float)device.Viewport.Height) * device.Viewport.Width), 720, 0, 0);
            
            MusicPlayer.AddSoundEffect("click", contentRepo.LoadSoundEffect("Sounds/click"));
            MusicPlayer.AddSoundEffect("hover", contentRepo.LoadSoundEffect("Sounds/hover"));

            Instance = this;
        }

        public void StartActivity(Activity a)
        {
            if (_currentActivity != null)
            {
                _currentActivity.OnDestroy();
            }

            if (a != null)
            {
                _currentActivity = a;
                _currentActivity.OnCreate();
            }
            else {
                OnActivitiesFinished();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_currentActivity != null)
            {
                _currentActivity.Update(gameTime);
                Checkmouse(gameTime);
#if __ANDROID__
                CheckTouch(gameTime);
#endif
            }
        }

        #region Mouse and Touch Input

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private MouseState clickedMouseState;
        private MouseState middleClickedMouseState;
        private TimeSpan downTime;

        private void Checkmouse(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState =
#if WPF
 _mouseInput.MouseState;
#else
               Mouse.GetState();
#endif
            int mouseX = currentMouseState.X;
            int mouseY = currentMouseState.Y;

            if(previousMouseState != currentMouseState) { 
                _currentActivity.OnCursorMove(
                    new Vector2(previousMouseState.X, previousMouseState.Y),
                    new Vector2(currentMouseState.X, currentMouseState.Y));
            }
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                downTime = downTime.Add(gameTime.ElapsedGameTime);
            }

            //
            //  Mouse down
            //
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                clickedMouseState = currentMouseState;
                _currentActivity.OnCursorDown(new Vector2(mouseX, mouseY));
                downTime = new TimeSpan();
            }

            //
            //  Clicked Event
            //
            if (currentMouseState.LeftButton == ButtonState.Released &&
                previousMouseState.LeftButton == ButtonState.Pressed
            )
            {
                Vector2 clickPoint = new Vector2(mouseX, mouseY);
                _currentActivity.OnCursorUp(clickPoint);
                if ((clickedMouseState.X - currentMouseState.X) == 0 &&
                    (clickedMouseState.Y - currentMouseState.Y) == 0 &&
                    (downTime.TotalMilliseconds < 1000) &&
                    _currentActivity.OnCursorClick(clickPoint))
                {
                    // Can stay empty for now. Crucial computation happes in the condition.
                    MusicPlayer.PlayEffect("click");
                }
            }

            //
            //  Mouse move with button down
            //
            if (previousMouseState.LeftButton == ButtonState.Pressed &&
                currentMouseState.LeftButton == ButtonState.Pressed)
            {
                _currentActivity.OnMoveGesture(new Vector2(currentMouseState.X - clickedMouseState.X, currentMouseState.Y - clickedMouseState.Y));
            }

            //
            // Resize started (Mouse Scroll)
            //
            if (previousMouseState.RightButton == ButtonState.Released &&
                currentMouseState.RightButton == ButtonState.Pressed)
            {
                middleClickedMouseState = currentMouseState;
            }

            if (//currentMouseState.MiddleButton == ButtonState.Pressed)
            previousMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
            {
                if (currentMouseState.LeftButton == ButtonState.Released)
                    _currentActivity.OnCursorDown(new Vector2(mouseX, mouseY));
                float origDist = previousMouseState.ScrollWheelValue / 50;
                float newDist = currentMouseState.ScrollWheelValue / 50;

                _currentActivity.OnResizeGesture(origDist, newDist);
            }

        }

        private TouchCollection currentTouchState;
        private TouchCollection previousTouchState;
        private TouchCollection clickedTouchState;
        private bool twoFingers = false;

#if __ANDROID__
		private GamePadState previousGamePad;
		private GamePadState currentGamePad;
#endif

        private void CheckTouch(GameTime gameTime)
        {
            previousTouchState = currentTouchState;
            OnTouchUpdate(currentTouchState = TouchPanel.GetState());



            if (previousTouchState.Count == 0 && twoFingers)
            {
                Console.WriteLine("No two fingers");
                twoFingers = false;
            }

            if (currentTouchState.Count == 1 &&
                previousTouchState.Count == 1 && 
                currentTouchState[0].Position != previousTouchState[0].Position)
            {
                _currentActivity.OnCursorMove(previousTouchState[0].Position, currentTouchState[0].Position);
            }

            float touchX = currentTouchState.Count > 0 ? (int)currentTouchState[0].Position.X : 0;
            float touchY = currentTouchState.Count > 0 ? (int)currentTouchState[0].Position.Y : 0;

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                downTime = downTime.Add(gameTime.ElapsedGameTime);
            }


            //
            // One finger on the screen Event
            //
            if (currentTouchState.Count == 1 &&
                (previousTouchState.Count == 0 /*|| previousTouchState.Count == 2*/))
            {
                clickedTouchState = currentTouchState;
                _currentActivity.OnCursorDown(new Vector2(touchX, touchY));
                downTime = new TimeSpan();
            }

            //
            // Click Event
            //
            float dist = 0;
            if (currentTouchState.Count == 0 &&
                previousTouchState.Count == 1 &&
                !twoFingers)
            {
                dist = Vector2.Distance(clickedTouchState[0].Position, previousTouchState[0].Position);
                Console.WriteLine(dist);
                _currentActivity.OnCursorUp(previousTouchState[0].Position);
                if (dist < 10 &&
                    downTime.TotalMilliseconds < 1000 &&
                    _currentActivity.OnCursorClick(previousTouchState[0].Position)
                )
                {
                    // Can stay empty for now. Crucial computation happes in the condition.
                    MusicPlayer.PlayEffect("click");
                }
            }

            //
            //  Touch moved 
            //
            if (clickedTouchState.Count == 1 &&
                         currentTouchState.Count == 1 && !twoFingers)
            {
                _currentActivity.OnMoveGesture(currentTouchState[0].Position - clickedTouchState[0].Position);
            }

            //
            // Resize started
            //
            if (currentTouchState.Count == 2 &&
                 previousTouchState.Count != 2)
            {
                twoFingers = true;
                clickedTouchState = currentTouchState;
            }

            //
            // Resize moved
            //
            if (previousTouchState.Count == 2 &&
                currentTouchState.Count == 2)
            {
                twoFingers = true;
                float origDist = Vector2.Distance(clickedTouchState[0].Position, clickedTouchState[1].Position);
                float newDist = Vector2.Distance(currentTouchState[0].Position, currentTouchState[1].Position);
                _currentActivity.OnResizeGesture(origDist, newDist);
            }
#if __ANDROID__
			previousGamePad = currentGamePad;
			currentGamePad = GamePad.GetState (PlayerIndex.One);


			if (previousGamePad != null) {
                if (currentGamePad.Buttons.Back == ButtonState.Released &&
                    previousGamePad.Buttons.Back == ButtonState.Pressed) {
                    //		Game1.Activity.OnBackPressed ();						
                }
            }
#endif
        }

        #endregion

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            Screen.ScreenWidth = Device.Viewport.Width;
            Screen.ScreenHeight = Device.Viewport.Height;

            if (_currentActivity != null)
            {
                _currentActivity.Draw(gameTime, spritebatch);
            }
        }

        public void Destroy() {
            MusicPlayer.Destroy();
        }
    }
}
