using LoopLib.World;
using Microsoft.Xna.Framework;
using OpenFeasyo.GameTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopLib
{
    public class Level : ILevel
    {
        private Random _rand;
        private float _starsFreq;
        private float _bombFreq;
        

        public bool Finished
        {
            get { throw new NotImplementedException(); }
        }


        public int LevelId
        {
            get;           
            set;
        }

        public int Score
        {
            get;
            set;
        }

        public int Speed
        {
            get;
            private set;
        }

        public Level(float starsFreq, float bombFreq, int speed)
        {
            _rand = new Random();
            _bombFreq = bombFreq;
            _starsFreq = starsFreq;
            Speed = speed;
        }

        private HashSet<int> _intermediatePositions = new HashSet<int>();

        public void GenerateElements(ElementManager manager, Tunnel.TunnelSectionCreatedEventArgs e)
        {
            _intermediatePositions.Clear();
            for (int i = 0; i < 10; i++)
            {
                int pos = _rand.Next(0, e.tunnelSection.NumSegments * e.tunnelSection.TunnelLengthInCells);
                if (_intermediatePositions.Contains(pos) || _rand.NextDouble() > _starsFreq)
                {
                    continue;
                }
                _intermediatePositions.Add(pos);

                Matrix transform = manager.GetNewElementTranform(
                    pos % e.tunnelSection.NumSegments,
                    pos / e.tunnelSection.NumSegments, e);

                TunnelObject obj = manager.StarFactory.GetElement(manager.Scene);
                obj.InitializeTransform(transform, Tunnel.GetTunnelOffset(e.tunnelSection.PhaseAtStart - transform.Translation.Z));
            }

            for (int i = 0; i < 10; i++)
            {

                int pos = _rand.Next(0, e.tunnelSection.NumSegments * e.tunnelSection.TunnelLengthInCells);
                if (_intermediatePositions.Contains(pos) || _rand.NextDouble() > _bombFreq)
                {
                    continue;
                }
                _intermediatePositions.Add(pos);

                Matrix transform = manager.GetNewElementTranform(
                    pos % e.tunnelSection.NumSegments,
                    pos / e.tunnelSection.NumSegments, e);

                TunnelObject obj = manager.BombFactory.GetElement(manager.Scene);
                obj.InitializeTransform(transform, Tunnel.GetTunnelOffset(e.tunnelSection.PhaseAtStart - transform.Translation.Z));
            }



        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spritebatch)
        {
            // Not used
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Not used
        }
    }
}
