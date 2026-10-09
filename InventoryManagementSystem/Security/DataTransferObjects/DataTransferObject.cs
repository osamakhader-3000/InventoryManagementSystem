
namespace SecurityApplicationPresentation.DataTransferObjects
{ 

    public class DataTransferObject<T> 
    {
        public T? Data { get; private set; }
        public int ResultCode { get; private set; }
        public string ResultMessage { get; private set; }

        public DataTransferObject(int resultCode)
        {
            ResultCode = resultCode;
            ResultMessage = string.Empty;
            Data = default;
        }

        public DataTransferObject(int httpCode, T? data)
        {
            ResultCode = httpCode;
            ResultMessage = string.Empty;
            Data = data;
        }

        public DataTransferObject(int httpCode, string resultMessage, T? data)
        {
            ResultCode = httpCode;
            ResultMessage = resultMessage;
            Data = data;
        }
    }
}
