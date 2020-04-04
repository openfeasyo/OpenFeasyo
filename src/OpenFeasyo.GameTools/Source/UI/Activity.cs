
namespace OpenFeasyo.GameTools.UI
{
    public class Activity : ComponentCollection
    {
        protected UIEngine _engine;
        public Activity(UIEngine engine) { _engine = engine; }

        public virtual void OnCreate() { }

        public virtual void OnDestroy() { }

        //public virtual void Update(GameTime gameTime) {
        //    Base
        //}



        protected void StartActivity(Activity a)
        {
            _engine.StartActivity(a);
        }
    }
}
