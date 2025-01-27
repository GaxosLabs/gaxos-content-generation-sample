using System;
using ContentGeneration.Models;
using Sample.Base;
using Sample.Common;

namespace Sample.Sword
{
    public class SelectSwordToggle : SelectGeneratedImageToggle
    {
        protected override void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate)
        {
            ProfileSettings.OnSwordChanged += changedDelegate;
        }

        protected override PublishedAsset GetCurrentGeneratedImage()
        {
            return ProfileSettings.sword;
        }

        protected override void SetCurrentGeneratedImage(PublishedAsset value)
        {
            ProfileSettings.sword = value;
        }
    }
}