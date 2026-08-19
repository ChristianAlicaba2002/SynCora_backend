namespace syncora_server.Exceptions;

public class FollowExceptions
{
    public class NotFoundReceiverId(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; } = status;
    }
}