using Mango.Web.Utility;

namespace Mango.Web.Models.Dto
{
    public class RequestDto
    {
        public ApiTypes ApiType { get; set; }
        public string url { get; set; }
        public object request { get; set; }
        public string token { get; set; }
    }
}
