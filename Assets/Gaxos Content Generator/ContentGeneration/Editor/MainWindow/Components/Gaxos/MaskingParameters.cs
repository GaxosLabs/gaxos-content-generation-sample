using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class MaskingParameters : VisualElementComponent, IParameters<GaxosMaskingParameters>
    {
        public new class UxmlFactory : UxmlFactory<MaskingParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription _hidePrompt = new() { name = "HidePrompt" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (MaskingParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        public bool hidePrompt
        {
            get => gaxosParametersElement.hidePrompt;
            set
            {
                gaxosParametersElement.hidePrompt = value;
                mask.style.display = value ? DisplayStyle.None : DisplayStyle.Flex;
                if (!value)
                {
                    maskRequired.style.visibility = Visibility.Hidden;
                }
            }
        }

        GaxosParametersElement gaxosParametersElement => this.Q<GaxosParametersElement>("gaxosParametersElement");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");
        ImageSelection mask => this.Q<ImageSelection>("mask");
        Label maskRequired => this.Q<Label>("maskRequiredLabel");

        public MaskingParameters()
        {
            gaxosParametersElement.OnCodeHasChanged = CodeHasChanged;
            maskRequired.style.visibility = Visibility.Hidden;
            CodeHasChanged();
        }

        public Action codeHasChanged { get; set; }

        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (updateUI)
            {
                maskRequired.style.visibility = Visibility.Hidden;
            }

            if (!gaxosParametersElement.Valid(updateUI))
            {
                return false;
            }

            if (mask.image == null)
            {
                if(updateUI)
                {
                    maskRequired.style.visibility = Visibility.Visible;
                }
                return false;
            }

            return true;
        }

        public void ApplyParameters(GaxosMaskingParameters gaxosParameters)
        {
            gaxosParameters.Mask = (Texture2D)mask.image;
            gaxosParametersElement.ApplyParameters(gaxosParameters);
        }

        public string GetCode()
        {
            return gaxosParametersElement.GetCode() +
                   "\t\tMask = <Texture2D object>,\n";
        }

        public void Show(Favorite favorite)
        {
            var gaxosMaskingParameters = favorite.GeneratorParameters.ToObject<GaxosMaskingParameters>();
            gaxosParametersElement.Show(gaxosMaskingParameters);
            generationOptions.Show(favorite.GenerationOptions);
            
            CodeHasChanged();
        }
    }
}