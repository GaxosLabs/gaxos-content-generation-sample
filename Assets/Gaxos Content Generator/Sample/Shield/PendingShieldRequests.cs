using Sample.Base;

namespace Sample.Shield
{
    public class PendingShieldRequests : PendingGenerateImageRequests
    {
        protected override string subject => NewShieldForm.ShieldSubject;
    }
}