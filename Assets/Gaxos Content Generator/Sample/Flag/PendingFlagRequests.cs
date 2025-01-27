using Sample.Base;

namespace Sample.Flag
{
    public class PendingFlagRequests : PendingGenerateImageRequests
    {
        protected override string subject => NewFlagForm.FlagSubject;
    }
}