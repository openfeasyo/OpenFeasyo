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
using System.Collections.Generic;

namespace LoopLib.World
{
    //
    // This class is a derivative work of the project published in the following website:
    // https://www.codeproject.com/Articles/471705/Celerity-Sensory-Overload
    //
    public class Tunnel
    {
        private List<TunnelSection> sections;
        private int numSections;
        public int NumSections { get { return numSections; } }
        public int numSegments;
        public float radius;
        private int sectionLength;
        public int SectionLength { get { return sectionLength; } }
        private int sectionPointer;
        public float CellSize { get { return this.sections[0].CellSize; } }
        public TunnelSection CurrentSection { get { return this.sections[this.sectionPointer]; } }
        //private Game game;

        //private float period;
        //public float Period { get { return period; } }
        private float currentPhase;
        public float CurrentPhase { get { return currentPhase; } }
        public float speed;

        public bool createObstacles = true;
        public float chaos;

        private Effect tunnelEffect;
        private Texture2D tunnelTexture;

        public delegate void TunnelSectionCreatedHandler(object sender, TunnelSectionCreatedEventArgs e);
        public class TunnelSectionCreatedEventArgs : EventArgs
        {
            public TunnelSection tunnelSection;
            public TunnelSectionCreatedEventArgs(TunnelSection tunnelSection)
            {
                this.tunnelSection = tunnelSection;
            }
        }
        public event TunnelSectionCreatedHandler TunnelSectionCreated;
        protected void OnTunnelSectionCreated(object sender, TunnelSectionCreatedEventArgs e)
        {
            if (TunnelSectionCreated != null)
            {
                TunnelSectionCreated(this, e);
            }
        }

        public delegate void TunnelSectionSetZCalledHandler(object sender, TunnelSectionSetZCalledEventArgs e);
        public class TunnelSectionSetZCalledEventArgs : EventArgs
        {
            public TunnelSection tunnelSection;
            public TunnelSectionSetZCalledEventArgs(TunnelSection tunnelSection)
            {
                this.tunnelSection = tunnelSection;
            }
        }
        public event TunnelSectionSetZCalledHandler TunnelSectionSetZCalled;
        protected void OnTunnelSectionSetZCalled(object sender, TunnelSectionSetZCalledEventArgs e)
        {
            if (TunnelSectionSetZCalled != null)
            {
                TunnelSectionSetZCalled(this, e);
            }
        }

        struct OffsetParameters
        {
            //don't go lower than the second decimal place without changing resolution too
            public static float xFreq = 0.5f;
            public static float xAmp = 10.0f;
            public static float xPhaseShift = 5.0f;
            public static float yFreq = 0.75f;
            public static float yAmp = 10.0f;
            //turns out it not quite straight forward to calculate period. if you play with x and y freq you have to work this out manually.
            //if the ratio between x and y freq turn out to be irrational it would be impossible to calculate without heavy iteration I believe.
            //will look into this further at a later date.
            public static float period = 1440.0f;

            //used when calculating the LCM of the frequency modifiers in order to find the period of the compound sine wave we've set up
            //this is done to make sure the tunnel loops correctly over the sine wave. The resolution should equal the value of the least significant decimal place
            public static decimal resolution = 0.01m;
        }

        public Tunnel(int LengthOfSection, Effect tunnelEffect, Texture2D tunnelTexture)//Game gameHandle, 
        {
            //create tunnel sections
            this.numSections = 3;
            this.numSegments = 10;
            this.radius = 10.0f;
            this.sectionPointer = 0;
            this.sectionLength = LengthOfSection;
            this.sections = new List<TunnelSection>();
            this.tunnelEffect = tunnelEffect;
            this.tunnelTexture = tunnelTexture;
            this.chaos = 0.0f;
            for (int i = 0; i < this.numSections; i++)
            {
                TunnelSection section = new TunnelSection(tunnelEffect, tunnelTexture)
                {
                    NumSegments = this.numSegments,
                    Radius = this.radius,
                    TunnelLengthInCells = sectionLength
                };
                section.ConstructTunnel();
                section.PhaseAtStart = i * (section.TunnelLengthInCells - 1) * section.CellSize;
                section.SetInitialZ();
                section.ConstructCurves();
                sections.Add(section);
            }
            this.currentPhase = 0.0f;
            this.speed = 1.0f;
        }

        public static Vector2 GetTunnelOffset(float Phase)
        {
            float toRadians = (float)(Math.PI / 180);
            float xOffset = (float)(Tunnel.OffsetParameters.xAmp * Math.Sin((Tunnel.OffsetParameters.xFreq * Phase + Tunnel.OffsetParameters.xPhaseShift) * toRadians));
            float yOffset = (float)(Tunnel.OffsetParameters.yAmp * Math.Sin(Tunnel.OffsetParameters.yFreq * Phase * toRadians));
            return new Vector2(xOffset, yOffset);
            //return new Vector2(0.0f, 0.0f);
        }

        public Vector3 GetTunnelCentrePos(float deltaPhase)
        {
            Vector2 offset = Tunnel.GetTunnelOffset(this.currentPhase + deltaPhase);
            return new Vector3(-offset.X, offset.Y, deltaPhase);
        }


        public static Vector3 GetTunnelDirection(float Phase)
        {
            float deltaPhase = 0.01f;
            Vector3 currentPosition = new Vector3(Tunnel.GetTunnelOffset(Phase), Phase);
            Vector3 newPosition = new Vector3(Tunnel.GetTunnelOffset(Phase + deltaPhase), Phase + deltaPhase);
            return Vector3.Normalize(newPosition - currentPosition);
            //return new Vector3(0.0f, 0.0f, 1.0f);
        }

        private static int deltaResolution = 10;
        public float DeltaPhase(GameTime gameTime)
        {
            Vector3 currentDirection = Tunnel.GetTunnelDirection(this.currentPhase);
            float distanceTravelled = (float)gameTime.ElapsedGameTime.TotalMilliseconds * this.speed / 1000.0f;
            float deltaDistance = distanceTravelled / (float)Tunnel.deltaResolution;

            Vector3 deltaPosition;
            Vector3 currentPosition = new Vector3(Tunnel.GetTunnelOffset(this.currentPhase), this.currentPhase);
            for (int i = 0; i < deltaResolution; i++)
            {
                deltaPosition = currentDirection * deltaDistance;
                Vector2 realXY = Tunnel.GetTunnelOffset(currentPhase + deltaPosition.Z);
                deltaPosition.X = realXY.X;
                deltaPosition.Y = realXY.Y;
                currentPosition += deltaPosition;
            }

            return currentPosition.Z - currentPhase;
        }

        private void IncrementSectionPointer()
        {
            this.sectionPointer++;
            if (this.sectionPointer == this.numSections) this.sectionPointer = 0;
        }

        private float IncrementPhase(float deltaPhase)
        {
            float newPhase = this.currentPhase + deltaPhase;
            if (newPhase >= Tunnel.OffsetParameters.period) newPhase = newPhase - Tunnel.OffsetParameters.period;
            return newPhase;
        }

        private float TrimPhase(float Phase)
        {
            if (Phase >= Tunnel.OffsetParameters.period)
                return Phase - Tunnel.OffsetParameters.period;
            else
                return Phase;
        }

        private void TranslateTunnelSections(float deltaPhase)
        {
            Matrix translation = Matrix.CreateTranslation(0.0f, 0.0f, deltaPhase);
            foreach (TunnelSection section in sections)
            {
                section.Transform(translation);
            }
        }

        private TunnelSection CreateSection(float PhaseAtStart)
        {
            TunnelSection section = new TunnelSection(tunnelEffect, tunnelTexture)
            {
                NumSegments = this.numSegments,
                Radius = this.radius,
                TunnelLengthInCells = this.sectionLength
            };
            section.ConstructTunnel();
            section.PhaseAtStart = PhaseAtStart;
            section.ConstructCurves();
            this.OnTunnelSectionCreated(this, new TunnelSectionCreatedEventArgs(section));
            return section;
        }

        private void ManageSections()
        {
            TunnelSection currentSection = sections[this.sectionPointer];
            if (currentSection.ZAtEnd >= 0.0f)
            {
                this.IncrementSectionPointer();
                float phaseAtEnd = 0.0f;
                float zAtEnd = 0.0f;
                switch (this.sectionPointer)
                {
                    case 0:
                        phaseAtEnd = this.TrimPhase(this.sections[1].PhaseAtEnd);
                        zAtEnd = this.sections[1].ZAtEnd;
                        this.sections[2] = this.CreateSection(phaseAtEnd);
                        this.sections[2].SetZ(zAtEnd);
                        this.OnTunnelSectionSetZCalled(this, new TunnelSectionSetZCalledEventArgs(this.sections[2]));
                        break;
                    case 1:
                        phaseAtEnd = this.TrimPhase(this.sections[2].PhaseAtEnd);
                        zAtEnd = this.sections[2].ZAtEnd;
                        this.sections[0] = this.CreateSection(phaseAtEnd);
                        this.sections[0].SetZ(zAtEnd);
                        this.OnTunnelSectionSetZCalled(this, new TunnelSectionSetZCalledEventArgs(this.sections[0]));
                        break;
                    case 2:
                        phaseAtEnd = this.TrimPhase(this.sections[0].PhaseAtEnd);
                        zAtEnd = this.sections[0].ZAtEnd;
                        this.sections[1] = this.CreateSection(phaseAtEnd);
                        this.sections[1].SetZ(zAtEnd);
                        this.OnTunnelSectionSetZCalled(this, new TunnelSectionSetZCalledEventArgs(this.sections[1]));
                        break;
                }
            }
        }

        public void Draw(GraphicsDevice Device, Matrix World, Matrix View, Matrix Projection, float FarClip)
        {
            foreach (TunnelSection section in sections)
                section.Draw(Device, World, View, Projection, FarClip);
        }

        public void Update(GameTime gameTime, float speed)
        {
            // TODO: Change the speed according to Chaos
            //this.chaos = chaos;
            //if (GlobalGameStates.GameState != Data.GameState.InGame)
            if(true)
            {
                this.speed = speed;
            }
            else
            {
                float targetSpeed = 20f + (float)(int)(chaos * 35f);
                if (this.speed < targetSpeed)
                {
                    // HACK: Technically this should should GameTime and velocity but this'll do for now
                    this.speed += 1f;
                }
                else
                {
                    if (this.speed > targetSpeed)
                    {
                        // HACK: Technically this should should GameTime and velocity but this'll do for now
                        this.speed -= 1f;
                    }
                }
            }

            //this.speed = 0.0f;

            float deltaPhase = this.DeltaPhase(gameTime);
            this.currentPhase = IncrementPhase(deltaPhase);
            TranslateTunnelSections(deltaPhase);
            this.ManageSections();
        }
    }
}
