using Sample.Base;

namespace Sample.Body
{
    public class PendingBodyRequests : PendingGenerateImageRequests
    {
        protected override string subject => NewBodyForm.BodySubject;
    }
}