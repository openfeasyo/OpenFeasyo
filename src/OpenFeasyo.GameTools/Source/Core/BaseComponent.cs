using Microsoft.Xna.Framework;

namespace OpenFeasyo.GameTools.Core
{
    public class BaseComponent
    {
        public SceneEntity ParentObject { get; set; }

        public virtual void OnUpdate(GameTime gametime) { }
    }
}
