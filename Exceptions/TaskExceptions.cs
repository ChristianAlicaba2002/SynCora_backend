namespace syncora_server.Exceptions;

public class TaskExceptions
{
     public class TaskNotFound(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; } = status;
    }
}