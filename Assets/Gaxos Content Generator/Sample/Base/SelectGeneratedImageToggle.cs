using System;
using ContentGeneration.Models;
using Sample.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Toggle))]
    public abstract class SelectGeneratedImageToggle : MonoBehaviour
    {
        protected abstract void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate);
        protected abstract PublishedAsset GetCurrentGeneratedImage();
        protected abstract void SetCurrentGeneratedImage(PublishedAsset value);
        
        void Awake()
        {
            var toggle = GetComponent<Toggle>();
            var parentPublishedImageRow = GetComponentInParent<PublishedImageRow>();
            parentPublishedImageRow.OnPublishedImageChanged += image => 
                toggle.isOn = image?.ID == GetCurrentGeneratedImage()?.ID;
            toggle.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    if (GetCurrentGeneratedImage()?.ID != parentPublishedImageRow.publishedAsset?.ID)
                    {
                        SetCurrentGeneratedImage(parentPublishedImageRow.publishedAsset);
                    }
                }
            });
            SubscribeToGeneratedImageChangedEvent(v =>
            {
                toggle.isOn = v?.ID == parentPublishedImageRow.publishedAsset?.ID;
            });
        }
    }
}