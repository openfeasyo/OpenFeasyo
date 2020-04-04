
namespace OpenFeasyo.GameTools.Core
{
    public interface SceneInterface
    {
        ObjectManager ObjectManager { get; }

        void Submit(Scene scene);

        void Remove(Scene scene);
    }
}
