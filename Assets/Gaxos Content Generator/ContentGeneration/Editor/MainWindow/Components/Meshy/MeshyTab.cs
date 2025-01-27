using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class MeshyTab: VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<MeshyTab, UxmlTraits>
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
