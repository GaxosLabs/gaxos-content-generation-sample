using System;
using ContentGeneration.Models;
using Sample.Base;
using Sample.Common;

namespace Sample.Body
{
    public class SelectBodyToggle : SelectGeneratedImageToggle
    {
        protected override void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate)
        {
            ProfileSettings.OnBodyChanged += changedDelegate;
        }

        protected override PublishedAsset GetCurrentGeneratedImage()
        {
            return ProfileSettings.body;
        }

        protected override void SetCurrentGeneratedImage(PublishedAsset value)
        {
            ProfileSettings.body = value;
        }
    }
}