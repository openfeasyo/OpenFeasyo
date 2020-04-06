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
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Bepu;
using System.Collections.Generic;

namespace OpenFeasyo.GameTools.Core
{
    public class SceneEntity :
#if WPF
        MonoGameControl.
#endif
        DrawableGameComponent
    {
        /// <summary> 
        /// Effect for drawing the boundong box
        /// </summary>
        private BasicEffect bbEffect;

        /// <summary>
        /// List containing all component to the object
        /// </summary>
        private List<BaseComponent> _components;

        /// <summary>
        /// Entity representation in the physics engine
        /// </summary>
        private Entity entity;

        public Entity Entity { get { return entity; } }

        public ICamera Camera { get; set; }
        /// <summary>
        /// Object name. Should be unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag saying whether the object should collide with other objects or not
        /// NOTE: The flag has to be set before adding the object to the Scene.
        /// </summary>
        public bool Collide { get; set; }

        public bool ShowBoundingBox { get; set; }

        /// <summary> 
        /// World coordinate transformation matrix
        /// </summary>
        public Matrix World
        {
            get { return MathConverter.Convert(entity.WorldTransform); }
            set { entity.WorldTransform = MathConverter.Convert(value); }
        }

        

        public SceneEntity(
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
            Game game,
            ICamera camera,
            Entity entity)
            : base(game)
        {
            this.entity = entity;
            this.Camera = camera;
            ShowBoundingBox = false;
            _components = new List<BaseComponent>(); 
        }

        public override void Update(GameTime gameTime)
        {
            if (!GameTools.IsPaused) { 
                foreach (BaseComponent component in _components)
                {
                    component.OnUpdate(gameTime);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw bounding box of this object
        /// Note: For now valid only for Box and Sphere.
        /// </summary>
        protected void DrawBoundingBox()
        {
            VertexPositionColor[] list = null;
            if (entity is BEPUphysics.Entities.Prefabs.Sphere)
            {
                list = new VertexPositionColor[8];
                Sphere sphere = (Sphere)entity;
                list[0] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(-sphere.Radius, -sphere.Radius, -sphere.Radius), Color.White);
                list[1] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(sphere.Radius, -sphere.Radius, -sphere.Radius), Color.White);
                list[2] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(sphere.Radius, -sphere.Radius, sphere.Radius), Color.White);
                list[3] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(-sphere.Radius, -sphere.Radius, sphere.Radius), Color.White);
                list[4] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(-sphere.Radius, sphere.Radius, -sphere.Radius), Color.White);
                list[5] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(sphere.Radius, sphere.Radius, -sphere.Radius), Color.White);
                list[6] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(sphere.Radius, sphere.Radius, sphere.Radius), Color.White);
                list[7] = new VertexPositionColor(MathConverter.Convert(sphere.Position) + new Vector3(-sphere.Radius, sphere.Radius, sphere.Radius), Color.White);
            }
            else if (entity is BEPUphysics.Entities.Prefabs.Box)
            {
                list = new VertexPositionColor[8];
                Box box = (Box)entity;

                list[0] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(-box.HalfWidth, -box.HalfHeight, -box.HalfLength), Color.White);
                list[1] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(box.HalfWidth, -box.HalfHeight, -box.HalfLength), Color.White);
                list[2] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(box.HalfWidth, -box.HalfHeight, box.HalfLength), Color.White);
                list[3] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(-box.HalfWidth, -box.HalfHeight, box.HalfLength), Color.White);
                list[4] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(-box.HalfWidth, box.HalfHeight, -box.HalfLength), Color.White);
                list[5] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(box.HalfWidth, box.HalfHeight, -box.HalfLength), Color.White);
                list[6] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(box.HalfWidth, box.HalfHeight, box.HalfLength), Color.White);
                list[7] = new VertexPositionColor(MathConverter.Convert(box.Position) + new Vector3(-box.HalfWidth, box.HalfHeight, box.HalfLength), Color.White);
            }

            if (list != null)
            {
                short[] indices = new short[8 * 3];

                // drawing a cube
                for (short i = 0; i < 4; i++)
                {
                    indices[i * 2] = i;
                    indices[i * 2 + 1] = (short)((i + 1) % 4);

                    indices[i * 2 + 8] = (short)(4 + i);
                    indices[i * 2 + 1 + 8] = (short)(((i + 1) % 4) + 4);

                    indices[i * 2 + 16] = i;
                    indices[i * 2 + 1 + 16] = (short)((i % 4) + 4);
                }

                if (bbEffect != null)
                {
                    bbEffect.View = Camera.View;
                    bbEffect.Projection = Camera.Projection;

                    foreach (EffectPass pass in bbEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                            PrimitiveType.LineList,
                            list,
                            0,  // vertex buffer offset to add to each element of the index buffer
                            list.Length,  // number of vertices in pointList
                            indices,  // the index buffer
                            0,  // first index element to read
                            12  // number of primitives to draw
                            );
                    }
                }
                else
                {
                    bbEffect = new BasicEffect(GraphicsDevice);
                    bbEffect.VertexColorEnabled = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if(ShowBoundingBox){
                DrawBoundingBox();
            }
            base.Draw(gameTime);
        }

        public void AddComponnet(BaseComponent component)
        {
            component.ParentObject = this;
            _components.Add(component);
        }

        public void RemoveComponnet(BaseComponent component)
        {
            _components.Remove(component);
            component.ParentObject = null;
        }
    }
}
