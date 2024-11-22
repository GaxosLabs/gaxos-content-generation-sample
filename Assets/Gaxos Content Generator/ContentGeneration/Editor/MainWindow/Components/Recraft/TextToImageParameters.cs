using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.Recraft;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Recraft
{
    public class TextToImageParameters : VisualElementComponent, IParameters<RecraftTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");
        public Action codeHasChanged { get; set; }

        public TextToImageParameters()
        {
            
        }
        
        public bool Valid()
        {
            throw new NotImplementedException();
        }

        public void ApplyParameters(RecraftTextToImageParameters parameters)
        {
            throw new NotImplementedException();
        }

        public string GetCode()
        {
            throw new NotImplementedException();
        }

        public void Show(Favorite generatorParameters)
        {
            throw new NotImplementedException();
        }
    }
}