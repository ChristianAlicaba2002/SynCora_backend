namespace syncora_server.Exceptions;

public class UserExceptions
{
    public class EmailAlreadyUsed(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int Status { get; } = status;
    }

    public class RequiredAllFields(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int Status { get; } = status;
    }
    public class EmailIsRequired(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int Status { get; } = status;
    }
    public class PasswordIsRequired(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int Status { get; } = status;
    }
    

    public class UserNotFound(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int Status { get; } = status;
    }
}
