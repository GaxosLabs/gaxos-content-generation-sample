using Sample.Base;

namespace Sample.Sword
{
    public class PendingSwordRequests : PendingGenerateImageRequests
    {
        protected override string subject => NewSwordForm.SwordSubject;
    }
}