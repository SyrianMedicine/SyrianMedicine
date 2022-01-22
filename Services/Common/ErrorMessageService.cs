namespace Services.Common
{

    public enum ErrorMessage
    {
        InternalServerError = 1,
        UnKnown
    }
    public static class ErrorMessageService
    {
        private static readonly Dictionary<ErrorMessage, string> errorMessage = new()
        {
            { (ErrorMessage)1, "Internal Server Error !" },
            { (ErrorMessage)2, "Something happen, please try again *_^" }
        };

        public static string GetErrorMessage(ErrorMessage error)
            => errorMessage[error];
    }
}