using System.Collections.Generic;
using ContentGeneration.Editor.MainWindow.Components;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.Gaxos_Content_Generator.ContentGeneration.Editor.MainWindow.Components.Recraft
{
    public class RecraftTab: VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<RecraftTab, UxmlTraits>
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