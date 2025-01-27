using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class ImageToMeshParameters : VisualElementComponent, IParameters<MeshyImageToMeshParameters>
    {
        public new class UxmlFactory : UxmlFactory<ImageToMeshParameters, UxmlTraits>
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
                var element = (ImageToMeshParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        ImageSelection image => this.Q<ImageSelection>("image");
        Label imageRequired => this.Q<Label>("imageRequired");
        Toggle enablePbr => this.Q<Toggle>("enablePbr");
        EnumField surfaceMode => this.Q<EnumField>("surfaceMode");

        public GenerationOptionsElement generationOptions => null;

        bool _hidePrompt;
        public bool hidePrompt { get; set; }

        public ImageToMeshParameters()
        {
            imageRequired.style.visibility = Visibility.Hidden;
            enablePbr.RegisterValueChangedCallback(_ => CodeHasChanged());
            surfaceMode.RegisterValueChangedCallback(_ => CodeHasChanged());
            image.OnChanged += CodeHasChanged;
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
                imageRequired.style.visibility = Visibility.Hidden;
            }

            if (image.image == null)
            {
                if(updateUI)
                {
                    imageRequired.style.visibility = Visibility.Visible;
                }
                return false;
            }

            return true;
        }

        public void ApplyParameters(MeshyImageToMeshParameters parameters)
        {
            parameters.Image = (Texture2D)image.image;
            parameters.EnablePbr = enablePbr.value;
            parameters.SurfaceMode = (SurfaceMode)surfaceMode.value;
        }

        public string GetCode()
        {
            return $"\t\tImage = <Texture2D object>,\n" +
                   $"\t\tEnablePbr = \"{enablePbr.value}\",\n" +
                   $"\t\tSurfaceMode = SurfaceMode.{surfaceMode.value}"; 
        }

        public void Show(Favorite favorite)
        {
            var meshyImageToMeshParameters = favorite.GeneratorParameters.ToObject<MeshyImageToMeshParameters>();
            
            enablePbr.value = meshyImageToMeshParameters.EnablePbr;
            surfaceMode.value = meshyImageToMeshParameters.SurfaceMode;
            CodeHasChanged();
        }
    }
}