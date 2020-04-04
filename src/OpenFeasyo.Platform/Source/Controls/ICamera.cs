using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFeasyo.Platform.Controls
{
    public abstract class PerspectiveCamera : ICamera
    {
        public static Matrix SceneProjection { get; set; }

        public float Distance { get; set; }
        public float Angle { get; set; }
        public float Height { get; set; }
        public Vector3 TargetPosition { get; set; }

        public Matrix Projection { get { return SceneProjection; } }
        public Matrix View
        {
            get
            {
                return Matrix.CreateScale(1f) * Matrix.CreateTranslation(0, 0, 0) * GetLookAt();
            }
        }

        protected abstract Matrix GetLookAt();
    }

    public interface ICamera
    {
        float Distance { get; set; }
        float Angle { get; set; }
        float Height { get; set; }
        Vector3 TargetPosition { get; set; }

        Matrix Projection { get; }
        Matrix View { get; }
    }
}
