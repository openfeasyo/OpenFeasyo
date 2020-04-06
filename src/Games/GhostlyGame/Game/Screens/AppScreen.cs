//using FMPlatformEmgExtension;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Threading;

//namespace Ghostly.Screens
//{
//    class AppScreen : Screen
//    {
//        private Thread _renderThread;

//        private IDevice _emgDevice;
//        private IEmgSensorInput _emgInput;



//        public AppScreen()
//        {

//            ConnectToDevice();

            

//        }

//        private void ConnectToDevice()
//        {
//            _gameState = GameState.DeviceConnecting;
//            //ShowLoadingPanel(true);
//            Dictionary<string, string> dict = new Dictionary<string, string>();

//            //if (Context.Instance.Device.Equals("Shimmer"))
//            //{
//            //this.statusBar.Text = "Connecting to Shimmer3.";
//            //this._emgDevice = new Shimmer3Device();
//            //}
//            //else if (Context.Instance.Device.Equals("FlexVolt"))
//            //{
//            //this.statusBar.Text = "Connecting to FlexVolt.";
//            //this._emgDevice = new FlexVoltDevice();
//            //}
//            //else if (Context.Instance.Device.Equals("FlexShield"))
//            //{
//            //this.statusBar.Text = "Connecting to FlexShield.";
//            this._emgDevice = new FlexShieldDevice();
//            //}
//            //else
//            //{
//            //this.statusBar.Text = "No device set.";
//            //}

//            if (this._emgDevice != null)
//            {
//                this._emgDevice.LoadDriver(dict);

//                if (this._emgDevice.IsLoaded)
//                {
//                    //this.statusBar.Text = "Device Training.";
//                    this._emgInput = _emgDevice.GamingInput as IEmgSensorInput;
//                    this._emgInput.TrainedChanged += _emgInput_TrainedChanged;
//                    _gameState = GameState.DeviceConnected;
//                }
//                else
//                {
//                    //ShowMessagePanel();
//                    _gameState = GameState.DeviceCannotConnect;
//                }
//            }
//        }

//        private void _emgInput_TrainedChanged(object sender, TrainedChangedEventArgs e)
//        {
//            //this.statusBar.Text = "Device Trained.";
//            //ShowLoadingPanel(false);
//            _gameState = GameState.DeviceTrained;
//        }


        


//        public override void Initialize() { }


//        public override void LoadContent(ContentManager content)
//        {
//            throw new NotImplementedException();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            throw new NotImplementedException();
//        }

//        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
//        {
//            throw new NotImplementedException();
//        }

//        public override void UnloadContent()
//        {
//            //closing = true;
//            if (this._emgDevice != null)
//            {
//                this._emgDevice.UnloadDriver();
//            //    Context.Instance.SaveAppSettings();
//            }
            
//        }

//    }
//}
