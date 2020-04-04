using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenFeasyo.GameTools.Bepu;
using System;

namespace OpenFeasyo.GameTools.Core
{
    /// <summary>
    /// Component that draws a model following the position and orientation of a BEPUphysics entity.
    /// </summary>
    public class SceneObject : SceneEntity

    {

        /// <summary>
        /// Entity that this model follows.
        /// </summary>
        Model model;
        /// <summary>
        /// Base transformation to apply to the model.
        /// </summary>
        public Matrix Transform;
        Matrix[] boneTransforms;
        

        /// <summary>
        /// Creates a new EntityModel.
        /// </summary>
        /// <param name="entity">Entity to attach the graphical representation to.</param>
        /// <param name="model">Graphical representation to use for the entity.</param>
        /// <param name="transform">Base transformation to apply to the model before moving to the entity.</param>
        /// <param name="game">Game to which this component will belong.</param>
        public SceneObject(String name, Entity entity, Model model, Matrix transform,
#if WPF
            MonoGameControl.
#else 
            Microsoft.Xna.Framework.
#endif
            Game game, ICamera camera)
            : base(game, camera, entity)
        {
            this.Name = name;
            this.model = model;
            this.Transform = transform;
            this.Collide = true;
            
            //Collect any bone transformations in the model itself.
            //The default cube model doesn't have any, but this allows the EntityModel to work with more complicated shapes.
            boneTransforms = new Matrix[model.Bones.Count];
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            }
        }

        
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            //Notice that the entity's worldTransform property is being accessed here.
            //This property is returns a rigid transformation representing the orientation
            //and translation of the entity combined.
            //There are a variety of properties available in the entity, try looking around
            //in the list to familiarize yourself with it.
            Matrix worldMatrix = Transform * MathConverter.Convert(Entity.WorldTransform);

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(1f, -1.2f, 0f);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f); // with green highlights
                    effect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
                    effect.EmissiveColor = new Vector3(0f, 0f, 0);

                    effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        
    }
}
