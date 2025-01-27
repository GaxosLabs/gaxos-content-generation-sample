using System;
using ContentGeneration.Models;
using Sample.Base;
using Sample.Common;

namespace Sample.Shield
{
    public class ShieldMaterialTexture : GeneratedImageMaterialTexture
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