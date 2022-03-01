namespace Services.Common
{
    public class ResponseService<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public ResponseService<T> SetMessage(string Message)
        {
            this.Message = Message;
            return this;
        }
        public ResponseService<T> SetStatus(string Status)
        {
            this.Status = Status;
            return this;
        }
        public ResponseService<T> SetData(T? Data)
        {
            this.Data = Data;
            return this;
        }
        public static ResponseService<T> GetExeptionResponse()
        {
            return new ResponseService<T>().SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError)).SetStatus(StatusCodes.InternalServerError.ToString());
        }
    }
}