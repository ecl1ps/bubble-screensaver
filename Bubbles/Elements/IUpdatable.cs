using System.Windows;

namespace Bubbles
{
    public interface IUpdatable
    {
        void Update(Size bounds, float tpf);
    }
}
