using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Models.Dto
{
    public class RequestDto
    {
        public ApiTypes ApiType { get; set; }
        public string url { get; set; }
        public object Data { get; set; }
        public string token { get; set; }
    }
}
