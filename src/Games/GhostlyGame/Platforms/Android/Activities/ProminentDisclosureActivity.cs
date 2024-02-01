
#if ANDROID
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpenFeasyo.GameTools.UI;
using System;

namespace GhostlyLib.Activities
{
    public class ProminentDisclosureActivity : OpenFeasyo.GameTools.UI.Activity
    {
        private bool? _permissionsGranted = null;

       
            public ProminentDisclosureActivity(UIEngine engine) : base(engine)
            {
                float cell = engine.Screen.ScreenHeight / 16;
                int fontSize = new int[] { 12, 24, 36, 48, 64 }[Math.Max(engine.Screen.FontSize - 1, 0)];

                Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
                backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
                backgroundImage.Position = Vector2.Zero;
                Components.Add(backgroundImage);

                Label infoLabel = new Label("Ghostly Game collects location data to enable discovery and ", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), GhostlyGame.MENU_FONT_COLOR);
                infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 2) - infoLabel.Size / 2;

                Label info2Label = new Label("connection to EMG sensors even when the app is closed or not in use.", engine.Content.LoadFont("Fonts/Ubuntu" + fontSize), GhostlyGame.MENU_FONT_COLOR);
                info2Label.Position = new Vector2(engine.Screen.ScreenMiddle.X, cell * 4) - info2Label.Size / 2;
                
                TextButton emgButton = new TextButton("Allow", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                emgButton.Clicked += EmgButton_Clicked;
                emgButton.CursorEntered += (object sender, EventArgs e) => { engine.MusicPlayer.PlayEffect("hover"); };
                emgButton.Position = engine.Screen.ScreenMiddle - emgButton.Size / 2 - new Vector2(-engine.Screen.ScreenMiddle.X / 2, 0);

                TextButton denyButton = new TextButton("Deny", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                denyButton.Clicked += (object sender, TextButton.ClickedEventArgs e) => { StartActivity(new InputSelectionActivity(_engine)); };
                denyButton.CursorEntered += (object sender, EventArgs e) => { engine.MusicPlayer.PlayEffect("hover"); };
                denyButton.Position = engine.Screen.ScreenMiddle - emgButton.Size / 2 - new Vector2(engine.Screen.ScreenMiddle.X / 2, 0);

                Image vubetrologoImage = new Image(_engine.Content.LoadTexture("textures/vubetrologo"));
                vubetrologoImage.Size = new Vector2((vubetrologoImage.Size.X / vubetrologoImage.Size.Y) * cell * 2, cell * 2);
                vubetrologoImage.Position = engine.Screen.ScreenMiddle + new Vector2(-vubetrologoImage.Size.X, engine.Screen.ScreenMiddle.Y * 2 / 3 - vubetrologoImage.Size.Y / 2);

                Image delucalogoImage = new Image(_engine.Content.LoadTexture("textures/delucafoundationlogo"));
                delucalogoImage.Size = new Vector2((delucalogoImage.Size.X / delucalogoImage.Size.Y) * cell * 2, cell * 2);
                delucalogoImage.Position = engine.Screen.ScreenMiddle + new Vector2(10, engine.Screen.ScreenMiddle.Y * 2 / 3 - delucalogoImage.Size.Y / 2);



                Components.Add(vubetrologoImage);
                Components.Add(delucalogoImage);
                Components.Add(infoLabel);
                Components.Add(info2Label);
                Components.Add(emgButton);
                Components.Add(denyButton);

            }



            private void EmgButton_Clicked(object sender, TextButton.ClickedEventArgs e)
            {
#if ANDROID
                GhostlyLib.MainActivity.Instance.RequestPermissions(new[] {
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.BluetoothAdmin,
                        Manifest.Permission.Bluetooth,
                        Manifest.Permission.BluetoothConnect,
                        Manifest.Permission.BluetoothScan
            }, 3, granted => { _permissionsGranted = granted; });
#else
            _permissionsGranted = true;
#endif
            }

            public override void Update(GameTime gameTime)
            {
                if (_permissionsGranted.HasValue)
                {
                    if (_permissionsGranted.Value)
                    {
                        StartActivity(new SelectSensorActivity(_engine));
                    }
                    else
                    {
                        // TODO room for improvement
                        _permissionsGranted = null;
                    }

                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    _engine.StartActivity(null);
                }
                base.Update(gameTime);
            }

        }
}