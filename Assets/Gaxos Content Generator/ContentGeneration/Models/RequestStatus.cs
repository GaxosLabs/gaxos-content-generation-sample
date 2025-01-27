using ContentGeneration.Helpers;

namespace ContentGeneration.Models
{
    public enum RequestStatus
    {
        Pending, Generated, Failed
    }
    
    internal class RequestStatusConverter : EnumJsonConverter<RequestStatus>
    {
    }
}