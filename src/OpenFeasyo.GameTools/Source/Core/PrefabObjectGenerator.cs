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
