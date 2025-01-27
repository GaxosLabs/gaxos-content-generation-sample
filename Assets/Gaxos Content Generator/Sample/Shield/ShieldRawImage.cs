using System;
using ContentGeneration.Models;
using Sample.Base;
using Sample.Common;

namespace Sample.Shield
{
    public class ShieldRawImage : GeneratedImageRawImage
    {
        protected override void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate)
        {
            ProfileSettings.OnShieldChanged += changedDelegate;
        }

        protected override PublishedAsset GetCurrentGeneratedImage()
        {
            return ProfileSettings.shield;
        }
    }
}