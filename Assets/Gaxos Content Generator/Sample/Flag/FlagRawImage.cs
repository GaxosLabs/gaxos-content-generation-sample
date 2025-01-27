using System;
using ContentGeneration.Models;
using Sample.Base;
using Sample.Common;

namespace Sample.Flag
{
    public class FlagRawImage : GeneratedImageRawImage
    {
        protected override void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate)
        {
            ProfileSettings.OnFlagChanged += changedDelegate;
        }

        protected override PublishedAsset GetCurrentGeneratedImage()
        {
            return ProfileSettings.flag;
        }
    }
}