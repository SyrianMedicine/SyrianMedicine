namespace Services.Common
{
    public class ResponseService<T>
    {
        public StatusCodes StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}