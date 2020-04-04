using BEPUphysics;
using Microsoft.Xna.Framework;
using OpenFeasyo.GameTools.Bepu;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenFeasyo.GameTools.Core
{
    public class Scene
    {
        private Space _space;
        private List<SceneEntity> _objs;

        public IReadOnlyCollection<SceneEntity> Objects { get { return new ReadOnlyCollection<SceneEntity>(_objs); } }

        public SceneInterface SceneInterface { get; set; }

        public Scene() {
            SceneInterface = null;
            _objs = new List<SceneEntity>();
            _space = new Space();
            _space.ForceUpdater.Gravity = MathConverter.Convert(new Vector3(0, -9.81f, 0));
        }

        public void Update(GameTime gameTime) {
            if (!GameTools.IsPaused) { 
                _space.Update();
            }
        }

        public void Submit(SceneEntity obj)
        {
            if (obj.Collide) { 
                _space.Add(obj.Entity);
            }
            _objs.Add(obj);
            if (SceneInterface != null) {
                SceneInterface.ObjectManager.Submit(obj);
            }
        }

        public void Remove(SceneEntity obj)
        {
            if (!_objs.Contains(obj)) return;
            if (obj.Collide)     {
                _space.Remove(obj.Entity);
            }
            _objs.Remove(obj);
            if (SceneInterface != null)
            {
                SceneInterface.ObjectManager.Remove(obj);
            }
        }

        public void Dispose() {
            foreach (SceneEntity obj in _objs)
            {
                if (obj.Collide) { 
                    _space.Remove(obj.Entity);
                }
            }
            _objs.Clear();
        }
    }
}
