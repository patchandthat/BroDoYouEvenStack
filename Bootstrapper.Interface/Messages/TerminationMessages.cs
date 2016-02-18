namespace Bootstrapper.Interface.Messages
{
    public class TerminationMessages
    {
        public class Success
        {
            public Success(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        public class Error
        {
            public Error(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }
    }
}