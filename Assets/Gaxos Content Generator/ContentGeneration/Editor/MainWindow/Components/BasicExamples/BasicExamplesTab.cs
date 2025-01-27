using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.BasicExamples
{
    public class BasicExamplesTab: VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<BasicExamplesTab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
    }
}
