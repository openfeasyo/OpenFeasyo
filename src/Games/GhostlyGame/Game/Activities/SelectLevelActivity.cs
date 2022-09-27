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
using OpenFeasyo.GameTools.UI;

namespace GhostlyLib.Activities
{
    public class SelectLevelActivity : OpenFeasyo.GameTools.UI.Activity
    {
        public SelectLevelActivity(UIEngine engine, int world) : base(engine) {

            
            Image backgroundImage = new Image(_engine.Content.LoadTexture("textures/ghostly/menu_background"));
            backgroundImage.Size = new Vector2(engine.Screen.ScreenWidth, engine.Screen.ScreenHeight);
            backgroundImage.Position = Vector2.Zero;
            Components.Add(backgroundImage);

            Label infoLabel = new Label("Select the level", engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), GhostlyGame.MENU_FONT_COLOR);
            infoLabel.Position = new Vector2(engine.Screen.ScreenMiddle.X - (infoLabel.Size.X / 2), engine.Screen.ScreenHeight * 0.10f - (GhostlyGame.MENU_BUTTON_FONT_SIZE/2));

            Components.Add(infoLabel);
            float verticalSpacing = engine.Screen.ScreenHeight * 0.01f;
            float horizontalSpacing = engine.Screen.ScreenWidth * 0.01f;
            float tileWidth = (engine.Screen.ScreenWidth * 0.9f) / 6 - horizontalSpacing;
            float tileHeight =(engine.Screen.ScreenHeight * 0.75f) / 5 - verticalSpacing;
            Vector2 offset = new Vector2(engine.Screen.ScreenWidth * 0.06f, engine.Screen.ScreenHeight * 0.20f);
            int levelNum = 1 + (world - 1) * 30;
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 6; x++) {
                    LevelSelectionButton level1Button = new LevelSelectionButton(levelNum.ToString(), engine.Content.LoadFont(GhostlyGame.MENU_BUTTON_FONT + GhostlyGame.MENU_BUTTON_FONT_SIZE), engine.Device);
                    level1Button.Level = levelNum;
                    level1Button.Clicked += (object sender, TextButton.ClickedEventArgs e) => {
                        StartActivity(new GamePlayActivity(engine, ((LevelSelectionButton)sender).Level,
                            "<?xml version=\"1.0\" encoding=\"utf - 8\"?><Configuration>"+
                            "<devices><device name=\"TrignoEmg\">"+
                                "<analyzers><analyzer file=\"C3DSerializer.dll\" /></analyzers>"+
                            "</device></devices>" +
                            "<bindings>"+
                                "<binding point=\"Jump/Swim\" zeroAngle=\"0\" sensitivity=\"1\" device=\"TrignoEmg\"><emgSensor device=\"TrignoEmg\" channel=\"0\"></emgSensor></binding>"+
                                "<binding point=\"Shoot\" zeroAngle=\"0\" sensitivity=\"1\" device=\"TrignoEmg\"><emgSensor device=\"TrignoEmg\" channel=\"1\"></emgSensor></binding>"+
                            "</bindings></Configuration>"
                            ));
                    };
                    level1Button.Position = /*engine.Screen.ToScreen*/(offset + new Vector2(x * (tileWidth + horizontalSpacing), y * (tileHeight + verticalSpacing)));
                    level1Button.Size = new Vector2(tileWidth, tileHeight);
                    levelNum++;
                    Components.Add(level1Button);
                }


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _engine.StartActivity(new SelectWorldActivity(_engine));
            }
        }

    }

    internal class LevelSelectionButton : TextButton 
    {
        public int Level { get; set; }

        internal LevelSelectionButton(string name, SpriteFont font, GraphicsDevice device):
            base(name,font,device) {


        }
    }
}