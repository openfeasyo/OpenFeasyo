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
using System;
using System.Linq;

namespace LoopLib.World
{
    //
    // This class is a derivative work of the project published in the following website:
    // https://www.codeproject.com/Articles/471705/Celerity-Sensory-Overload
    //
    public class TunnelSection
    {
        private VertexPositionColorTexture[] vertices;
        private short[] indices;

        public float Radius { get; set; } //the radius of the tunnel walls
        public int NumSegments { get; set; } //the number of segments in the wall. 5 would make a pentagonal tunnel.
        public int TunnelLengthInCells { get; set; } //number of rings of vertices in the tunnel section
        private float cellSize; //distance between the rings of vertices
        public float CellSize { get { return cellSize; } }
        public float PhaseAtStart { get; set; }
        public float PhaseAtEnd { get { return PhaseAtStart + (this.cellSize * (this.TunnelLengthInCells - 1)); } }
        public float ZAtEnd { get { return vertices[vertices.GetUpperBound(0)].Position.Z; } }
        public float ZAtStart { get { return vertices[0].Position.Z; } }

        public Effect tunnelEffect;
        public Texture2D tunnelTexture;
        private Game game;

        public TunnelSection(Effect tunnelEffect, Texture2D tunnelTexture)
        {
            //default values
            this.Radius = 10.0f;
            this.NumSegments = 10;
            this.TunnelLengthInCells = 30;

            //game = gameHandle;
            this.tunnelEffect = tunnelEffect;// game.Content.Load<Effect>("shader");
            this.tunnelTexture = tunnelTexture;
        }

        public void ConstructTunnel()
        {
            cellSize = CalculateSectionSize();
            ConstructVertices();
            ConstructIndices();
            ConstructTextureCoords();
        }

        private float CalculateSectionSize()
        {
            Vector3 point1 = new Vector3(0.0f, this.Radius, 0.0f);
            Vector3 point2 = Vector3.Transform(point1, Matrix.CreateRotationZ(2 * (float)Math.PI / NumSegments));
            return Vector3.Distance(point1, point2);
        }

        private void ConstructVertices()
        {
            int numVertex = NumSegments * TunnelLengthInCells;
            vertices = new VertexPositionColorTexture[numVertex];
            float sectionAngle = 2 * (float)Math.PI / NumSegments;
            int vertexCounter = 0;

            for (int i = 0; i < TunnelLengthInCells; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    Matrix rotationMatrix = Matrix.CreateRotationZ(j * sectionAngle);
                    vertices[vertexCounter].Position = Vector3.Transform(new Vector3(0.0f, this.Radius, 0.0f), rotationMatrix);
                    vertices[vertexCounter].Position.Z = -cellSize * i;
                    vertexCounter++;
                }
            }
        }

        private void ConstructIndices()
        {
            int indexCount = TunnelLengthInCells * NumSegments * 6;
            indices = new short[indexCount];
            int indexCounter = 0;

            for (int i = 0; i < vertices.GetUpperBound(0) - NumSegments; i += NumSegments)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    //triangle 1
                    indices[indexCounter] = (short)(i + j);
                    indices[indexCounter + 1] = (short)(i + j + NumSegments);
                    indices[indexCounter + 2] = (short)(i + j + 1);
                    if (j == NumSegments - 1) indices[indexCounter + 2] = (short)i;
                    //triangle 2
                    if (j < NumSegments - 1)
                    {
                        indices[indexCounter + 3] = (short)(i + j + 1);
                        indices[indexCounter + 4] = (short)(i + j + NumSegments);
                        indices[indexCounter + 5] = (short)(i + j + NumSegments + 1);
                    }
                    else
                    {
                        indices[indexCounter + 3] = (short)(i + j + NumSegments);
                        indices[indexCounter + 4] = (short)(i + j + 1);
                        indices[indexCounter + 5] = (short)(i);
                    }

                    indexCounter += 6;
                }
            }
        }

        private void ConstructTextureCoords()
        {
            int vertexCounter = 0;
            for (int i = 0; i < TunnelLengthInCells; i++)
            {
                for (int j = 0; j < NumSegments; j++)
                {
                    vertices[vertexCounter].TextureCoordinate = new Vector2((float)(j % 2), (float)(i % 2));
                    vertexCounter++;
                }
            }
        }


        public void ConstructCurves()
        {
            Vector2 offset;
            for (int i = 0; i <= vertices.GetUpperBound(0); i++)
            {
                offset = Tunnel.GetTunnelOffset(this.PhaseAtStart - vertices[i].Position.Z + this.ZAtStart);
                vertices[i].Position.X += offset.X;
                vertices[i].Position.Y += offset.Y;
            }
        }

        public void SetInitialZ()
        {
            for (int i = 0; i <= vertices.GetUpperBound(0); i++)
            {
                vertices[i].Position.Z -= this.PhaseAtStart;
            }
        }

        public void SetZ(float zAtStart)
        {
            for (int i = 0; i <= vertices.GetUpperBound(0); i++)
            {
                vertices[i].Position.Z += zAtStart;
            }
        }

        public void Transform(Matrix transformationMatrix)
        {
            for (int i = 0; i <= this.vertices.GetUpperBound(0); i++)
            {
                vertices[i].Position = Vector3.Transform(vertices[i].Position, transformationMatrix);
            }
        }

        public void Draw(GraphicsDevice Device, Matrix World, Matrix View, Matrix Projection, float FarClip)
        {
            tunnelEffect.Parameters["World"].SetValue(World);
            tunnelEffect.Parameters["View"].SetValue(View);
            tunnelEffect.Parameters["Projection"].SetValue(Projection);
            tunnelEffect.Parameters["Color"].SetValue(new Vector4( 1.0f, 1.0f,1.0f, 1f ));
            tunnelEffect.Parameters["TunnelTexture"].SetValue(tunnelTexture);
            tunnelEffect.Parameters["FarClip"].SetValue(FarClip);
            tunnelEffect.CurrentTechnique = tunnelEffect.Techniques["Tunnel"];

            

            RasterizerState rasterizerState1 = new RasterizerState()
            {
                CullMode = CullMode.None,
                MultiSampleAntiAlias = false,
                FillMode = FillMode.Solid
            };
            DepthStencilState depthState = new DepthStencilState()
            {
                DepthBufferEnable = true
            };
            Device.RasterizerState = rasterizerState1;
            Device.DepthStencilState = depthState;
            foreach (EffectPass pass in tunnelEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                                this.vertices,
                                                                0,
                                                                this.vertices.Count(),
                                                                this.indices,
                                                                0,
                                                                this.indices.Count() / 3);
            }

        }

    }
}
