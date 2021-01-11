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
using OpenFeasyo.GameTools.Core;

namespace OpenFeasyo.GameTools.Components2D
{
    public class InfoPanel
    {
        //
        //  Internal Content of the library
        //
        
        public ILevel CurrentLevel { get; set; }

        public string LevelLabel { get; set; }
        public string ScoreLabel { get; set; }
        public string ScoreUnit { get; set; }

        private SpriteFont[] _font = new SpriteFont[5];
        private SpriteFont[] _fontIcons = new SpriteFont[5];
        private GraphicsDevice _device;
        private Matrix _projection;
        private Vector2 _scoreLabelPos = new Vector2(0.01f, 0.01f);
        private Vector2 _shadowOffest = new Vector2(0.002f, 0.002f);
        private Screen _screen;


        public InfoPanel(ContentRepository repo, Screen screen)
        {
            LevelLabel = "Level:";
            ScoreLabel = "Score:";
            ScoreUnit = "";

            _font[0] = repo.LoadFont("Fonts/Ubuntu12");
            _font[1] = repo.LoadFont("Fonts/Ubuntu24");
            _font[2] = repo.LoadFont("Fonts/Ubuntu36");
            _font[3] = repo.LoadFont("Fonts/Ubuntu48");
            _font[4] = repo.LoadFont("Fonts/Ubuntu64");
            _screen = screen;
            _projection = Matrix.CreateOrthographic(3f, 2f, 0.1f, 300);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            string level = LevelLabel != null ? LevelLabel + " " + (CurrentLevel == null ? "--" : CurrentLevel.LevelId.ToString()) : "";
            string score = ScoreLabel != null ? ScoreLabel + " " + (CurrentLevel == null ? "--" : CurrentLevel.Score.ToString()) + " " + ScoreUnit : "";
            DrawText(spritebatch,level + "  " + score,
                _scoreLabelPos);
        }

        private void DrawText(SpriteBatch spritebatch, string text, Vector2 pos)
        {
            SpriteFont f = _font[_screen.FontSize];

            spritebatch.DrawString(f, " " + text + "  ", _screen.ToScreen(_screen.TopLeft + pos + _shadowOffest), Color.Black);
            spritebatch.DrawString(f, " " + text + "  ", _screen.ToScreen(_screen.TopLeft + pos), Color.White);
        }


    }
}
