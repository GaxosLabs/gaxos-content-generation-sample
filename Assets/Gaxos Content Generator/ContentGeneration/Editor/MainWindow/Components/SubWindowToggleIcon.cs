using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class SubWindowToggleIcon : SubWindowToggle
    {
        public new class UxmlFactory : UxmlFactory<SubWindowToggleIcon, UxmlTraits>
        {
        }

        public new class UxmlTraits : SubWindowToggle.UxmlTraits
        {
        }

        VisualElement toggleInput => this.Q<VisualElement>(className: "unity-toggle__input");

        public override string subWindowName
        {
            get => base.subWindowName;
            set
            {
                base.subWindowName = value;
                toggleInput.style.backgroundImage = icon.style.backgroundImage;
            }
        }
    }
}