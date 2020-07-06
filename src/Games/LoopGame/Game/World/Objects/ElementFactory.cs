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
using OpenFeasyo.GameTools.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LoopLib.World.Objects
{
    public class ElementFactory<T> where T: TunnelObject
    {
        public static float CLEANING_DISTANCE = 5;
        public static float COLLISION_CHECKING_DISTANCE = 20;

        public ReadOnlyCollection<T> Objects { get { return new ReadOnlyCollection<T>(_usedObjects); } }

        private ElementManager _manager;
        private Queue<T> _freeObjects;
        private List<T> _usedObjects;
        private Func<ContentRepository,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game, ICamera, T> _creator;
        private ContentRepository _repo;
        private
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game _game;
        private ICamera _camera;

        public ElementFactory(int initCount, ContentRepository repo,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game game, ICamera camera, Func<ContentRepository,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif            
            Game,ICamera, T> creator, ElementManager manager)
        {
            _freeObjects = new Queue<T>();
            _usedObjects = new List<T>();
            _creator = creator;
            _repo = repo;
            _game = game;
            _camera = camera;
            _manager = manager;

            for(int i = 0; i < initCount; i++){
                T obj = creator(repo,game,camera);
                obj.Visible = false;
                _freeObjects.Enqueue(obj);
            }

            
        }

        public T GetElement(Scene scene){
            T obj = null;
            if (_freeObjects.Count == 0) {
                T o = _creator(_repo, _game, _camera);
                o.Visible = false;
                _freeObjects.Enqueue(o);
            }
            obj = _freeObjects.Dequeue();
            _usedObjects.Add(obj);
            obj.Visible = true;
            scene.Submit(obj);
            return obj;
        }

        

        

        public void Update(GameTime gameTime, Tunnel tunnel, Scene scene,Player player)
        {
            foreach (T obj in _usedObjects.ToList())
            {
                if (obj.World.Translation.Z < CLEANING_DISTANCE) {
                    RecycleElement(obj);
                    scene.Remove(obj);
                } else {
                    obj.World = obj.World * Matrix.CreateTranslation(0.0f, 0.0f, - tunnel.DeltaPhase(gameTime));
                    
                    if (obj.World.Translation.Z < COLLISION_CHECKING_DISTANCE) {
                        obj.Entity.CollisionInformation.UpdateBoundingBox(0);
                        player.Entity.CollisionInformation.UpdateBoundingBox(0);
                        HandleCollisions(obj, player, scene);
                    }
                }
            }
        }

        public void DestroyAll(Scene scene) {
            foreach (T obj in _usedObjects.ToList())
            {
                RecycleElement(obj);
                scene.Remove(obj);
            }
        }

        private void HandleCollisions(T obj, Player player, Scene scene)
        {
            if (!Collide(obj, player)) { 
                return; 
            }
            
            
            if(obj.OnCollision(player,_manager)){
                RecycleElement(obj);
                scene.Remove(obj);
            }
        }

        private static bool Collide(SceneEntity e1, SceneEntity e2) { 
            return e1.Entity.CollisionInformation.BoundingBox.Intersects(
                e2.Entity.CollisionInformation.BoundingBox);
        }

        private void RecycleElement(T obj){
            _usedObjects.Remove(obj);
            _freeObjects.Enqueue(obj);
            obj.Visible = false;
        }

 
    }
}
