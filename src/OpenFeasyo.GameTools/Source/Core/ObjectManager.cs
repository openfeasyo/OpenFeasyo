#if WPF
    using MonoGameControl;
#else
    using Microsoft.Xna.Framework;
#endif

namespace OpenFeasyo.GameTools.Core
{
    public class ObjectManager
    {
        private GameComponentCollection collection;

        public ObjectManager(GameComponentCollection objs) {
            collection = objs;
        }

        public void Submit(SceneEntity obj)
        {
            collection.Add(obj);
        }

        public void Remove(SceneEntity obj)
        {
            collection.Remove(obj);
        }
    }
}
