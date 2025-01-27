using ContentGeneration.Models;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public interface IGeneratorVisualElement
    {
        Generator generator { get; }
        void Show(Favorite favorite);
    }
}