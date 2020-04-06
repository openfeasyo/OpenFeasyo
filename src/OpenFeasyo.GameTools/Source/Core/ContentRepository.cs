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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace OpenFeasyo.GameTools.Core
{
    public class ContentRepository
    {
        private Dictionary<string, Model> _allModels;
        
        private 
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
                                    Game _game;

        public 
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
                                    Game Game { get { return _game; } }

        public ContentRepository(
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
            Game game)
        {
            _allModels = new Dictionary<string, Model>();
            _game = game;
        }

        public Model LoadModel(string name) {
            if (!_allModels.ContainsKey(name))
            {
                Model model = _game.Content.Load<Model>(name);
                _allModels.Add(name,model);
                return model;
            }
            return _allModels[name];
        }

        public Texture2D LoadTexture(string name)
        {
            return _game.Content.Load<Texture2D>(name);
        }

        public Song LoadSong(string name)
        {
            return _game.Content.Load<Song>(name);
        }

        public SoundEffect LoadSoundEffect(string name)
        {
            return _game.Content.Load<SoundEffect>(name);
        }



        public Effect LoadEffect(string name)
        {
            return _game.Content.Load<Effect>(name);
        }

        public SpriteFont LoadFont(string name)
        {
            return _game.Content.Load<SpriteFont>(name);
        }

        public PrefabObjectGenerator LoadPrefab(Camera camera, string prefabName) {
            Model model = null;
            if (!_allModels.ContainsKey(prefabName)) {
                model = _game.Content.Load<Model>(prefabName);
            }
            if (model == null) {
                throw new ApplicationException("ContentRepository: Could not load resource: " + prefabName);
            }
            return new PrefabObjectGenerator(_game,camera, model);
        }
    }
}
