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
using LoopLib.World.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Core;
using OpenFeasyo.GameTools.Effects;
using OpenFeasyo.GameTools.Effects.Particles;
using System;

namespace LoopLib.World
{
    
    public class ElementManager
    {
        public ParticleSystem FireParticles { get; set; }

        public ParticleSystem ExplosionParticles { get; set; }

        public ParticleSystem SmokeParticles { get; set; }

        public ParticleSystem StarsParticles { get; set; }

        public MusicPlayer Music { get; private set; }

        private SceneInterface _sceneInterface;
        private ContentRepository _repo;
        private
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game _game;
        private ICamera _camera;
        private Tunnel _tunnel;
        private Player _player;
        
        

        public ElementFactory<Bomb> BombFactory { get; private set; }
        public ElementFactory<Star> StarFactory { get; private set; }

        public ElementManager(ContentRepository repo, SceneInterface si,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
            Game game, 
            ICamera camera, Tunnel tunnel, Player player, MusicPlayer music) {
                ExplosionParticles = new ParticleSystem(game, game.Content, GetExplosionSettings());
                ExplosionParticles.Initialize();
                FireParticles = new ParticleSystem(game, game.Content, GetFireSettings());
                FireParticles.Initialize();
                SmokeParticles = new ParticleSystem(game, game.Content, GetSmokeSettings());
                SmokeParticles.Initialize();
                StarsParticles = new ParticleSystem(game, game.Content, GetStarsSettings());
                StarsParticles.Initialize();
        
            BombFactory = new ElementFactory<Bomb>(50,repo,game,camera, (ContentRepository r,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
                Game g, ICamera c) => new Bomb(r,g,c),this);
            StarFactory = new ElementFactory<Star>(50, repo, game, camera, (ContentRepository r,
#if WPF
 MonoGameControl.
#else
        Microsoft.Xna.Framework.
#endif
                Game g, ICamera c) => new Star(r, g, c),this);
            Music = music;
            Scene = new Scene();
            Scene.Submit(player);
            _sceneInterface = si;
            _sceneInterface.Submit(Scene);
            
            _repo = repo;
            _game = game;
            
            _camera = camera;
            _tunnel = tunnel;
            _player = player;

            _tunnel.TunnelSectionCreated += TunnelSectionCreated;
            _tunnel.TunnelSectionSetZCalled += TunnelSectionSetZCalled;
        }

        public Scene Scene { get; private set; }

        private Level _level;
        public Level Level {
            get { return _level; }
            set {
                _level = value;
                if (_level == null) {
                    DestroyAllObjects();        
                }
            } 
        }

        
        
        private void TunnelSectionCreated(object sender, Tunnel.TunnelSectionCreatedEventArgs e)
        {
            if (Level != null) {
                Level.GenerateElements(this, e);
            }
        }

        public void DestroyAllObjects() {
            StarFactory.DestroyAll(Scene);
            BombFactory.DestroyAll(Scene);
        }

        public void Unload() {
            _sceneInterface.Remove(Scene);
        }

        public Matrix GetNewElementTranform(int section, int pos, Tunnel.TunnelSectionCreatedEventArgs e)
        {
            float angle = ((float)(2 * Math.PI) / (float)e.tunnelSection.NumSegments) * section;
            angle += (float)Math.PI / (float)e.tunnelSection.NumSegments;
            
            float cellSize = _tunnel.CellSize;
            return Matrix.CreateTranslation(new Vector3(0.0f,
                (float)Math.Sqrt(e.tunnelSection.Radius * e.tunnelSection.Radius - (cellSize / 2) * (cellSize / 2)) - cellSize / 2, 
                (float)pos * -e.tunnelSection.CellSize + e.tunnelSection.CellSize / 2))
                * Matrix.CreateRotationZ(angle);
        }
        

        public void Update(GameTime gameTime) {
            StarFactory.Update(gameTime, _tunnel, Scene,_player);
            BombFactory.Update(gameTime, _tunnel, Scene,_player);

            ExplosionParticles.Update(gameTime);
            FireParticles.Update(gameTime);
            SmokeParticles.Update(gameTime);
            StarsParticles.Update(gameTime);
        }

        public void DrawParticles(GameTime gametime) {

            SmokeParticles.SetCamera(_camera.View, _camera.Projection);
            SmokeParticles.Draw(gametime); 
            ExplosionParticles.SetCamera(_camera.View, _camera.Projection);
            ExplosionParticles.Draw(gametime);
            FireParticles.SetCamera(_camera.View, _camera.Projection);
            FireParticles.Draw(gametime);
            
            StarsParticles.SetCamera(_camera.View, _camera.Projection);
            StarsParticles.Draw(gametime);
        }
        
        void TunnelSectionSetZCalled(object sender, Tunnel.TunnelSectionSetZCalledEventArgs e)
        {
            foreach (TunnelObject obj in BombFactory.Objects)
            {
                obj.InitializeZPosition(-e.tunnelSection.ZAtStart);
            }
            foreach (TunnelObject obj in StarFactory.Objects)
            {
                obj.InitializeZPosition(-e.tunnelSection.ZAtStart);
            }
        }


        #region Settings of particle systems


        private ParticleSettings GetExplosionSettings()
        {
            ParticleSettings settings = new ParticleSettings();
            settings.BlendState = BlendState.Additive;
            settings.TextureName = "Textures/explosion";
            settings.MaxParticles = 100;
            settings.Duration = new TimeSpan(0, 0, 2);
            settings.DurationRandomness = 1;
            settings.EmitterVelocitySensitivity = 1;
            settings.MinHorizontalVelocity = 2;
            settings.MaxHorizontalVelocity = 3;
            settings.MinVerticalVelocity = -2;
            settings.MaxVerticalVelocity = 2;
            settings.Gravity = Vector3.Zero;
            settings.EndVelocity = 0;
            settings.MinColor = Color.FromNonPremultiplied(0x80, 0x80, 0x80, 0xFF);
            settings.MaxColor = Color.FromNonPremultiplied(0xA9, 0xA9, 0xA9, 0xFF);
            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;
            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;
            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;
            return settings;
        }

        private ParticleSettings GetFireSettings()
        {
            ParticleSettings settings = new ParticleSettings();
            settings.BlendState = BlendState.Additive;
            settings.TextureName = "Textures/fire";
            settings.MaxParticles = 2400;
            settings.Duration = new TimeSpan(0, 0, 2);
            settings.DurationRandomness = 1;
            settings.EmitterVelocitySensitivity = 1;
            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1.5f;
            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 1;
            settings.Gravity = new Vector3(0,15,0);
            settings.EndVelocity = 1;
            settings.MinColor = Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0x0A);
            settings.MaxColor = Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0x28);
            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;
            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1;
            settings.MinEndSize = 1;
            settings.MaxEndSize = 4;
            return settings;
        }

        private ParticleSettings GetSmokeSettings()
        {
            ParticleSettings settings = new ParticleSettings();
            settings.BlendState = BlendState.NonPremultiplied;
            settings.TextureName = "Textures/smoke";
            settings.MaxParticles = 600;
            settings.Duration = new TimeSpan(0, 0, 10);
            settings.DurationRandomness = 1;
            settings.EmitterVelocitySensitivity = 1;
            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1.5f;
            settings.MinVerticalVelocity = 1;
            settings.MaxVerticalVelocity = 2;
            settings.Gravity = new Vector3(-20,-5,-5);
            settings.EndVelocity = 0.075f;
            settings.MinColor = Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xFF);
            settings.MaxColor = Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xFF);
            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;
            settings.MinStartSize = 1f;
            settings.MaxStartSize = 2;
            settings.MinEndSize = 50;
            settings.MaxEndSize = 100;
            return settings;
        }

        private ParticleSettings GetStarsSettings()
        {
            ParticleSettings settings = new ParticleSettings();
            settings.BlendState = BlendState.Additive;
            settings.TextureName = "Textures/explosion";
            settings.MaxParticles = 100;
            settings.Duration = new TimeSpan(0, 0, 2);
            settings.DurationRandomness = 1;
            settings.EmitterVelocitySensitivity = 1;
            settings.MinHorizontalVelocity = 2;
            settings.MaxHorizontalVelocity = 3;
            settings.MinVerticalVelocity = -2;
            settings.MaxVerticalVelocity = 2;
            settings.Gravity = Vector3.Zero;
            settings.EndVelocity = 0;
            settings.MinColor = Color.FromNonPremultiplied(0x80, 0x80, 0x80, 0xFF);
            settings.MaxColor = Color.FromNonPremultiplied(0xA9, 0xA9, 0xA9, 0xFF);
            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;
            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;
            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;
            return settings;
        }

        #endregion
    }
}
