using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GhostlyLib
{
    public class _3DCamera
    {
        private static float angle = 0;
        private static float distance = 6.5f;
        private static float minZoom = 4.5f;
        private static float maxZoom = 6.5f;

        public static Matrix View { get; set; }
        public static Matrix World { get; set; }
        public static Matrix Projection { get; set; }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            // World matrix (identity)
            World = Matrix.Identity;

            // Simple camera view
            View = Matrix.CreateLookAt(
                new Vector3(0, 0, distance),    // Camera position
                Vector3.Zero,                   // Look at origin
                Vector3.Up                      // Up direction
            );

            // Projection matrix (perspective)
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                graphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );
        }

        public static void UpdateView(float angleChange, float distanceChange, float x, float y)
        {
            angle = angleChange;
            distance += distanceChange;

            if (distance >= maxZoom) distance = maxZoom;
            if (distance <= minZoom) distance = minZoom;

            Vector3 up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ(angle));
            View = Matrix.CreateLookAt(
                    new Vector3(x, y, distance),    // Camera position
                    new Vector3(x, y, 0),           // Look at origin
                    up                              // Up direction
                );
        }
    }
}