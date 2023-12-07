namespace Mango.Services.OrderAPI.Models.Dto
{
    public class ResponseDto
    {
        public string StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }
        public string Error { get; set; } = "";
    }
}
