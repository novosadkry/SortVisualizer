using SFML.Graphics;
using SFML.Window;

namespace SortVisualizer
{
    public class Window : RenderWindow
    {
        public event Action<Window> Tick;

        public Window(string title, VideoMode mode)
            : base(mode, title) { }

        public void Init()
        {
            Closed += (_, _) => Close();
        }

        public void Run()
        {
            while (IsOpen)
            {
                DispatchEvents();
                Clear();

                Tick(this);

                Display();
            }
        }
    }
}
