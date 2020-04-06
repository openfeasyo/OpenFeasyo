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
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Bepu;

namespace OpenFeasyo.GameTools.Core
{
    public class PrefabObjectGenerator
    {
        private Model _template;

        private Camera _camera;

        private
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
            Game _game;

        public PrefabObjectGenerator(
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
            Game game, Camera camera, Model template)
        {
            _template = template;
            _game = game;
            _camera = camera;
        }

        public SceneObject CreateObject<T>() {
            //
            //  This should be outside in a conf file eventually
            //  ... loaded based on a model type and conf.
            //
            int cubeSize = 10;
            float cubeScale = 0.25f;
            float offset = 1f;

            //  end of conf.
            Box box = new Box(MathConverter.Convert(new Vector3(-10, 30, 0)), cubeSize, cubeSize, cubeSize, 0.1f);

            return new SceneObject("Box",box, _template, Matrix.CreateScale(box.Width * cubeScale, box.Height * cubeScale, box.Length * cubeScale) * Matrix.CreateTranslation(offset+0.1f, offset-1.8f, offset), _game, _camera);
        }
    }
}
