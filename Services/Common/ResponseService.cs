namespace Services.Common
{
    public class ResponseService<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}