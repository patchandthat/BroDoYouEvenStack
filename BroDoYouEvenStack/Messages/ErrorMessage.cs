namespace BroDoYouEvenStack.Messages
{
    class ErrorMessage
    {
        public ErrorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
